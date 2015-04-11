using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	// TODO: The filter parameter is redundant to the CanNavigateTo method.
	// The CanNavigateTo method is quite handy to use in the router, the filter Func is handy in the vm implementation.
	// If we use only the filter Func, the router wouldn't know which vm could effectively be navigated to, so we would need a different mechanism
	// to communicate this. Therefore I think, we should leave the CanNavigateTo method as long as there is no better idea.
	public static class ReactiveViewModelExtensions
	{
		public static IObservable<F2F.ReactiveNavigation.ViewModel.INavigationParameters> WhenNavigatedTo(this ReactiveViewModel This)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.MainThreadScheduler);
		}

		public static IObservable<F2F.ReactiveNavigation.ViewModel.INavigationParameters> WhenNavigatedTo(
			this ReactiveViewModel This,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, bool> filter)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.MainThreadScheduler)
					.Where(filter);
		}

		public static IDisposable WhenNavigatedTo(
			this ReactiveViewModel This,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, bool> filter,
			Action<F2F.ReactiveNavigation.ViewModel.INavigationParameters> syncAction)
		{
			return
				This.WhenNavigatedTo(filter)
					.Do(syncAction)
					.Catch<F2F.ReactiveNavigation.ViewModel.INavigationParameters, Exception>(ex =>
					{
						This._thrownNavigationExceptions.OnNext(ex);
						return Observable.Return<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IObservable<F2F.ReactiveNavigation.ViewModel.INavigationParameters> WhenNavigatedToAsync(this ReactiveViewModel This)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.TaskpoolScheduler);
		}

		public static IDisposable WhenNavigatedToAsync(
			this ReactiveViewModel This,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, bool> filter,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, Task> asyncAction,
			Action<F2F.ReactiveNavigation.ViewModel.INavigationParameters> syncAction)
		{
			return
				This.WhenNavigatedToAsync()
					.Where(filter)
					.Do(_ => This._asyncNavigating.OnNext(true))
					.SelectMany(async p =>
						{
							await asyncAction(p);
							This._asyncNavigating.OnNext(false);
							return p;
						})
					.ObserveOn(RxApp.MainThreadScheduler)
					.Do(syncAction)
					.Catch<F2F.ReactiveNavigation.ViewModel.INavigationParameters, Exception>(ex =>
					{
						This._thrownNavigationExceptions.OnNext(ex);
						This._asyncNavigating.OnNext(false);
						return Observable.Return<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IDisposable WhenNavigatedToAsync<T>(
			this ReactiveViewModel This,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, bool> filter,
			Func<F2F.ReactiveNavigation.ViewModel.INavigationParameters, Task<T>> asyncAction,
			Action<F2F.ReactiveNavigation.ViewModel.INavigationParameters, T> syncAction)
		{
			return
				This.WhenNavigatedToAsync()
					.Where(filter)
					.SelectMany(async p =>
					{
						var result = await asyncAction(p);
						This._asyncNavigating.OnNext(false);
						return new
						{
							Result = result,
							Parameters = p
						};
					})
					.ObserveOn(RxApp.MainThreadScheduler)
					.Do(p => syncAction(p.Parameters, p.Result))
					.Catch<object, Exception>(ex =>
					{
						This._thrownNavigationExceptions.OnNext(ex);
						This._asyncNavigating.OnNext(false);
						return Observable.Return<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IObservable<F2F.ReactiveNavigation.ViewModel.INavigationParameters> WhenClosed(this ReactiveViewModel This)
		{
			return
				This.Closed
					.ObserveOn(RxApp.MainThreadScheduler);
		}

		public static IDisposable WhenClosed(
			this ReactiveViewModel This,
			Action<F2F.ReactiveNavigation.ViewModel.INavigationParameters> syncAction)
		{
			return
				This.WhenClosed()
					.Do(syncAction)
					.Catch<F2F.ReactiveNavigation.ViewModel.INavigationParameters, Exception>(ex =>
					{
						This._thrownNavigationExceptions.OnNext(ex);
						return Observable.Return<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(null);
					})
					.Subscribe();
		}
	}
}