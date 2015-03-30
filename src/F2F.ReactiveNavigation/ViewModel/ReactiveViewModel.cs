using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive.Subjects;

namespace F2F.ReactiveNavigation.ViewModel
{
	public class ReactiveViewModel : ReactiveObject, IHaveTitle
	{
		private string _title;
		private ObservableAsPropertyHelper<bool> _isBusy;

		public ReactiveViewModel()
		{	
		}

		// Implements the asynchronous initialization pattern with rx. Initialization is done on a background thread
		// TODO: Think about making this return Task! This is a single result, not a stream!
		// TODO: Think about how we can make this object busy during initialization
		public IObservable<Unit> Initialize()
		{
			return Observable.Start(() =>
			{
				NavigateTo = 
					ReactiveCommand.CreateAsyncObservable(
						p => NavigatedTo(p as INavigationParameters), 
						RxApp.MainThreadScheduler);

				Init();

				_isBusy =
					BusyObservables()
						.CombineLatest()
						.Select(bs => bs.Any(b => b))
						.Merge(NavigateTo.IsExecuting)
						.ToProperty(this, x => x.IsBusy, false);
				
			}, RxApp.TaskpoolScheduler);
		}

		protected virtual void Init()
		{
		}

		// implemented synchronously, since this decision should be made fast!
		internal protected virtual bool CanNavigateTo(INavigationParameters parameters)
		{
			return true;
		}

		// implemented synchronously, since CanClose should only ever ask the user, if she is ok with closing.
		internal protected virtual bool CanClose(INavigationParameters parameters)
		{
			return true;
		}

		internal protected virtual IEnumerable<IObservable<bool>> BusyObservables()
		{
			yield return Observable.Return(false);
		}

		// think of making this method return a task
		internal protected virtual IObservable<Unit> NavigatedTo(INavigationParameters parameters)
		{
			return Observable.Return(Unit.Default);
		}

		public bool IsBusy
		{
			get { return _isBusy != null ? _isBusy.Value : true; }	// this is tricky. If it is not yet set, we are still initalizing, so we return true --> we have busy indication during async init ! Awesome!
		}
		
		internal ReactiveCommand<Unit> NavigateTo { get; set; }

		public string Title
		{
			get { return _title; }
			set { this.RaiseAndSetIfChanged(ref _title, value); }
		}		
	}
}
