using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation;
using dbc = System.Diagnostics.Contracts;
using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System.Reactive;
using F2F.ReactiveNavigation.WPF.Sample.Controller;

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
			LongRunningOperation = ReactiveCommand.CreateAsyncTask(_ => Task.Delay(2000));
			Task.Delay(2000).Wait();
		}

		protected override IEnumerable<IObservable<bool>> BusyObservables()
		{
			yield return LongRunningOperation.IsExecuting;
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

		// TODO: Maybe this can always execute in the main thread scheduler, so we can make it return void/Task
		// and handle scheduling in the base class
		protected override IObservable<Unit> NavigatedTo(INavigationParameters parameters)
		{
			return Observable.Start(() =>
			{
				if (!_initialized && !parameters.IsUserNavigation())
				{
					_value = parameters.Get<int>("value");
					_initialized = true;

					this.Title = _controller.LoadTitle(_value);
				}
			}, RxApp.MainThreadScheduler);
		}

		protected override bool CanClose(INavigationParameters parameters)
		{
			return true;
		}

	}
}