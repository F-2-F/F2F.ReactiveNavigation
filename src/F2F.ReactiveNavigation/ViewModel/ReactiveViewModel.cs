using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	public class ReactiveViewModel : ReactiveObject, IInitializeAsync, IHaveTitle, ISupportBusyIndication
	{
		private string _title;
		private ObservableAsPropertyHelper<bool> _isBusy;

		public ReactiveViewModel()
		{
		}

		public Task InitializeAsync()
		{
			return Observable.Start(() =>
			{
				NavigateTo =
					ReactiveCommand.CreateAsyncObservable(
						p => NavigatedTo(p as INavigationParameters),
						RxApp.MainThreadScheduler);

				Initialize();

				_isBusy =
					BusyObservables()
						.CombineLatest()
						.Select(bs => bs.Any(b => b))
						.Merge(NavigateTo.IsExecuting)
						.ToProperty(this, x => x.IsBusy, false);
			}, RxApp.TaskpoolScheduler).ToTask();
		}

		public string Title
		{
			get { return _title; }
			set { this.RaiseAndSetIfChanged(ref _title, value); }
		}

		// this is tricky. If it is not yet set, we are still initalizing, so we return true --> we have busy indication during async init ! Awesome!
		public bool IsBusy
		{
			get { return _isBusy != null ? _isBusy.Value : true; }
		}

		internal ReactiveCommand<Unit> NavigateTo { get; private set; }

		protected virtual void Initialize()
		{
		}

		// implemented synchronously, since this decision should be made fast!
		protected internal virtual bool CanNavigateTo(INavigationParameters parameters)
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

		// think of making this method return a task
		protected internal virtual IObservable<Unit> NavigatedTo(INavigationParameters parameters)
		{
			return Observable.Return(Unit.Default);
		}
	}
}