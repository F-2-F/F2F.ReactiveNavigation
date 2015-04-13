using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	public class ReactiveViewModel : ReactiveObject, IInitializeAsync, IHaveTitle, ISupportBusyIndication
	{
		private interface INavigationCall
		{
			INavigationParameters Parameters { get; }
		}

		private class NavigateToCall : INavigationCall
		{
			public INavigationParameters Parameters { get; set; }
		}

		private class CloseCall : INavigationCall
		{
			public INavigationParameters Parameters { get; set; }
		}

		private string _title;
		private ObservableAsPropertyHelper<bool> _isBusy;

		private readonly Subject<INavigationCall> _navigation = new Subject<INavigationCall>();
		internal readonly Subject<bool> _asyncNavigating = new Subject<bool>();
		internal readonly ScheduledSubject<Exception> _thrownExceptions;

		private readonly IObserver<Exception> DefaultExceptionHandler =
			Observer.Create<Exception>(ex =>
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}

				RxApp.MainThreadScheduler.Schedule(() =>
				{
					throw new Exception(
						"An OnError occurred on an ReactiveViewModel, that would break the observables. To prevent this, Subscribe to the ThrownExceptions property of your objects",
						ex);
				});
			});

		public ReactiveViewModel()
		{
			_thrownExceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, DefaultExceptionHandler);
		}

		public Task InitializeAsync()
		{
			return Observable.Start(() =>
			{
				Initialize();

				_isBusy =
					BusyObservables
						.Concat(new[] { _asyncNavigating })
						.CombineLatest()
						.Select(bs => bs.Any(b => b))
						.Catch<bool, Exception>(ex =>
						{
							_thrownExceptions.OnNext(ex);
							return Observable.Return(false);
						})
						.ToProperty(this, x => x.IsBusy, false);
			}, RxApp.TaskpoolScheduler).ToTask();
		}

		internal IObservable<INavigationParameters> NavigatedTo
		{
			get
			{
				return _navigation
					.OfType<ReactiveViewModel.NavigateToCall>()
					.Select(c => c.Parameters);
			}
		}

		internal IObservable<INavigationParameters> Closed
		{
			get
			{
				return _navigation
					.OfType<ReactiveViewModel.CloseCall>()
					.Select(c => c.Parameters);
			}
		}

		public IObservable<Exception> ThrownExceptions
		{
			get { return _thrownExceptions.Merge(base.ThrownExceptions); }
		}

		public string Title
		{
			get { return _title; }
			set { this.RaiseAndSetIfChanged(ref _title, value); }
		}

		// this is tricky. If it is not yet set, we are still initalizing, so we return true --> we have busy indication during async init!
		public bool IsBusy
		{
			get { return _isBusy != null ? _isBusy.Value : true; }
		}

		protected internal virtual IEnumerable<IObservable<bool>> BusyObservables
		{
			get { yield return Observable.Return(false); }
		}

		protected internal virtual void Initialize()
		{
		}

		protected internal virtual bool CanNavigateTo(INavigationParameters parameters)
		{
			return true;
		}

		internal void NavigateTo(INavigationParameters parameters)
		{
			_navigation.OnNext(new NavigateToCall() { Parameters = parameters });
		}

		// implemented synchronously, since CanClose should only ever ask the user, if she is ok with closing.
		protected internal virtual bool CanClose(INavigationParameters parameters)
		{
			return true;
		}

		internal void Close(INavigationParameters parameters)
		{
			_navigation.OnNext(new CloseCall() { Parameters = parameters });
		}
	}
}