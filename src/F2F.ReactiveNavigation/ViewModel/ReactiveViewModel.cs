using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Diagnostics;

namespace F2F.ReactiveNavigation.ViewModel
{
	public class ReactiveViewModel : ReactiveObject, IInitializeAsync, IHaveTitle, ISupportBusyIndication
	{
		private string _title;
		private ObservableAsPropertyHelper<bool> _isBusy;
		internal readonly Subject<INavigationParameters> _navigateTo = new Subject<INavigationParameters>();
		internal readonly Subject<bool> _asyncNavigating = new Subject<bool>();
		internal readonly ScheduledSubject<Exception> _thrownNavigationExceptions;
		internal readonly ScheduledSubject<Exception> _thrownBusyExceptions;

		private readonly IObserver<Exception> DefaultNavigationExceptionHandler = 
			Observer.Create<Exception>(ex => 
			{
                if (Debugger.IsAttached) 
				{
                    Debugger.Break();
                }

                RxApp.MainThreadScheduler.Schedule(() => 
				{
                    throw new Exception(
                        "An OnError occurred on an ReactiveViewModel navigation request, that would break the navigation. To prevent this, Subscribe to the ThrownNavigationExceptions property of your objects",
                        ex);
                });
			});

		private readonly IObserver<Exception> DefaultBusyExceptionHandler =
			Observer.Create<Exception>(ex =>
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}

				RxApp.MainThreadScheduler.Schedule(() =>
				{
					throw new Exception(
						"An OnError occurred on an ReactiveViewModel busy observable, that would break the busy indication. To prevent this, Subscribe to the ThrownBusyExceptions property of your objects",
						ex);
				});
			});

		public ReactiveViewModel()
		{
			_thrownNavigationExceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, DefaultNavigationExceptionHandler);
			_thrownBusyExceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, DefaultBusyExceptionHandler);
		}

		public Task InitializeAsync()
		{
			return Observable.Start(() =>
			{
				Initialize();

				_isBusy =
					BusyObservables()
						.Concat(new [] { _asyncNavigating })
						.CombineLatest()
						.Select(bs => bs.Any(b => b))
						.Catch<bool, Exception>(ex =>
						{
							_thrownBusyExceptions.OnNext(ex);
							return Observable.Return(false);
						})
						.ToProperty(this, x => x.IsBusy, false);

			}, RxApp.TaskpoolScheduler).ToTask();
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

		public IObservable<Exception> ThrownNavigationExceptions
		{
			get { return _thrownNavigationExceptions.AsObservable(); }
		}

		public IObservable<Exception> ThrownBusyExceptions
		{
			get { return _thrownBusyExceptions.AsObservable(); }
		}

		internal void NavigateTo(INavigationParameters parameters)
		{
			_navigateTo.OnNext(parameters);
		}
		
		internal protected virtual void Initialize()
		{
		}

		internal protected virtual bool CanNavigateTo(INavigationParameters parameters)
		{
			return true;
		}

		// implemented synchronously, since CanClose should only ever ask the user, if she is ok with closing.
		protected internal virtual bool CanClose(INavigationParameters parameters)
		{
			return true;
		}

		protected internal virtual IEnumerable<IObservable<bool>> BusyObservables()
		{
			yield return Observable.Return(false);
		}
	}
}