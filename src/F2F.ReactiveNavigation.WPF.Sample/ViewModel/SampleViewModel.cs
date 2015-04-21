using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
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
		private int _value;
		private string _targetValue;

		public SampleViewModel(INavigate router, ISampleController controller)
		{
			if (router == null)
				throw new ArgumentNullException("router", "router is null.");
			if (controller == null)
				throw new ArgumentNullException("controller", "controller is null.");

			_router = router;
			_controller = controller;

			LongRunningOperation = ReactiveCommand.CreateAsyncTask(_ => Task.Delay(2000));

			GoToTarget = ReactiveCommand
				.CreateAsyncTask(_ => _router.RequestNavigate<SampleViewModel>(
					NavigationParameters.Create().Add("value", Convert.ToInt32(TargetValue))));
		}

		protected override void Initialize()
		{
			this.WhenNavigatedToAsync(
				filter: p => !_initialized && !p.IsUserNavigation(),
				asyncAction: p => Task.Delay(2000),
				syncAction: p =>
				{
					Value = p.Get<int>("value");
					_initialized = true;
					Title = _controller.LoadTitle(_value);
				});

			//this.WhenNavigatedTo()
			//	.Where(p => !_initialized && !p.IsUserNavigation())
			//	.DoAsync(p => { })
			//	.Do(p => { })
			//	.Subscribe();

			Task.Delay(2000).Wait();
		}

		protected override IEnumerable<IObservable<bool>> BusyObservables
		{
			get { yield return LongRunningOperation.IsExecuting; }
		}

		public int Value
		{
			get { return _value; }
			set { this.RaiseAndSetIfChanged(ref _value, value); }
		}

		public string TargetValue
		{
			get { return _targetValue; }
			set { this.RaiseAndSetIfChanged(ref _targetValue, value); }
		}

		public ReactiveCommand<Unit> LongRunningOperation { get; protected set; }

		public ReactiveCommand<Unit> GoToTarget { get; protected set; }

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