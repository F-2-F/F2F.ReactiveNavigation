using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF.Sample.Controller;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF.Sample.ViewModel
{
	public class SampleViewModel : ReactiveViewModel
	{
		private readonly INavigate _router;
		private readonly ISampleController _controller;

		private bool _initialized;

		public SampleViewModel(INavigate<Regions.TabRegion> router, ISampleController controller)
		{
			if (router == null)
				throw new ArgumentNullException("router", "router is null.");
			if (controller == null)
				throw new ArgumentNullException("controller", "controller is null.");

			_router = router;
			_controller = controller;
		}

		protected override async Task Initialize()
		{
			await base.Initialize();
			this.WhenNavigatedTo()
				.Where(p => !_initialized && !p.IsUserNavigation())
				.DoAsync(_ => Task.Delay(2000))
				.Do(p =>
					{
						Value = p.Get<int>("value");
						_initialized = true;
						Title = _controller.LoadTitle(_value);
					})
				.Subscribe();

			Task.Delay(2000).Wait();	// intentionally block to see busy indication during initialization		
		}

		protected override IEnumerable<IObservable<bool>> BusyObservables
		{
			get { yield return LongRunningOperation.IsExecuting; }
		}

		private int _value;

		public int Value
		{
			get { return _value; }
			set { this.RaiseAndSetIfChanged(ref _value, value); }
		}

		private string _targetValue;

		public string TargetValue
		{
			get { return _targetValue; }
			set { this.RaiseAndSetIfChanged(ref _targetValue, value); }
		}

		private ReactiveCommand<Unit> _longRunningOperation;

		public ReactiveCommand<Unit> LongRunningOperation
		{
			get
			{
				if (_longRunningOperation == null)
					_longRunningOperation = ReactiveCommand.CreateAsyncTask(_ => Task.Delay(2000));
				return _longRunningOperation;
			}
		}

		private ReactiveCommand<Unit> _goToTarget;

		public ReactiveCommand<Unit> GoToTarget
		{
			get
			{
				if (_goToTarget == null)
				{
					_goToTarget = ReactiveCommand
						.CreateAsyncTask(_ => _router.RequestNavigate<SampleViewModel>(
							NavigationParameters.Create().Add("value", Convert.ToInt32(TargetValue))));
				}

				return _goToTarget;
			}
		}

		protected override bool CanNavigateTo(INavigationParameters parameters)
		{
			return parameters.Get<int>("value") == _value;
		}

		protected override bool CanClose(INavigationParameters parameters)
		{
			return true;
		}
	}
}