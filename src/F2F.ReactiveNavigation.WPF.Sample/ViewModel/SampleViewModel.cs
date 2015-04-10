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
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.WPF.Sample.ViewModel
{
	public class SampleViewModel : ReactiveViewModel
	{
		private bool _initialized;
		private int _value;
		private readonly ISampleController _controller;

		public SampleViewModel(ISampleController controller)
		{
			dbc.Contract.Requires<ArgumentNullException>(controller != null, "controller must not be null");

			_controller = controller;
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

			LongRunningOperation = ReactiveCommand.CreateAsyncTask(_ => Task.Delay(2000));
			Task.Delay(2000).Wait();
		}

		protected override IEnumerable<IObservable<bool>> BusyObservables
		{
			get { yield return LongRunningOperation.IsExecuting; }
		}

		public ReactiveCommand<Unit> LongRunningOperation { get; protected set; }

		public int Value
		{
			get { return _value; }
			set { this.RaiseAndSetIfChanged(ref _value, value); }
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