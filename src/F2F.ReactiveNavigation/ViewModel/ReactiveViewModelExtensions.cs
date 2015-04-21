using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	public static class IObservableExtensions
	{
		public static IObservable<T> TryDo<T>(this IObservable<T> This, Action<T> action, Action<Exception> @catch)
		{
			return TryDo<T>(This, action, @catch, () => Observable.Return(default(T)));
		}

		public static IObservable<T> TryDo<T>(this IObservable<T> This, Action<T> action, Action<Exception> @catch, Func<IObservable<T>> fallback)
		{
			return
				This.ObserveOn(RxApp.MainThreadScheduler)
					.Do(action)
					.Catch<T, Exception>(ex =>
					{
						@catch(ex);
						return fallback();
					});
		}

		public static IObservable<T> TryDoAsync<T>(this IObservable<T> This, Func<T, Task> action, Action<Exception> @catch)
		{
			return TryDoAsync<T>(This, action, @catch, () => Observable.Return(default(T)));
		}

		public static IObservable<T> TryDoAsync<T>(this IObservable<T> This, Func<T, Task> action, Action<Exception> @catch, Func<IObservable<T>> fallback)
		{
			return
				This.ObserveOn(RxApp.TaskpoolScheduler)
					.SelectMany(async p =>
					{
						await action(p);
							
						return p;
					})
					.Catch<T, Exception>(ex =>
					{
						@catch(ex);
						return fallback();
					});
		}

		
	}

	// TODO: The filter parameter is redundant to the CanNavigateTo method.
	// The CanNavigateTo method is quite handy to use in the router, the filter Func is handy in the vm implementation.
	// If we use only the filter Func, the router wouldn't know which vm could effectively be navigated to, so we would need a different mechanism
	// to communicate this. Therefore I think, we should leave the CanNavigateTo method as long as there is no better idea.
	public static class ReactiveViewModelExtensions
	{
		public static IDisposable WhenNavigatedTo<T>(this ReactiveViewModel This, Func<ReactiveViewModel, IObservable<T>> getNavigationObservable)
		{
			return getNavigationObservable(This).Subscribe();
		}

		public static IObservable<INavigationParameters> WhenNavigatedTo(this ReactiveViewModel This)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.MainThreadScheduler);
		}

		public static IObservable<INavigationParameters> WhenNavigatedTo(
			this ReactiveViewModel This,
			Func<INavigationParameters, bool> filter)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.MainThreadScheduler)
					.Where(filter);
		}

		public static IDisposable WhenNavigatedTo(
			this ReactiveViewModel This,
			Func<INavigationParameters, bool> filter,
			Action<INavigationParameters> syncAction)
		{
			return
				This.WhenNavigatedTo(filter)
					.Do(syncAction)
					.Catch<INavigationParameters, Exception>(ex =>
					{
						This._thrownExceptions.OnNext(ex);
						return Observable.Return<INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IObservable<INavigationParameters> WhenNavigatedToAsync(this ReactiveViewModel This)
		{
			return
				This.NavigatedTo
					.ObserveOn(RxApp.TaskpoolScheduler);
		}

		public static IDisposable WhenNavigatedToAsync(
			this ReactiveViewModel This,
			Func<INavigationParameters, bool> filter,
			Func<INavigationParameters, Task> asyncAction,
			Action<INavigationParameters> syncAction)
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
					.Catch<INavigationParameters, Exception>(ex =>
					{
						This._thrownExceptions.OnNext(ex);
						This._asyncNavigating.OnNext(false);
						return Observable.Return<INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IDisposable WhenNavigatedToAsync<T>(
			this ReactiveViewModel This,
			Func<INavigationParameters, bool> filter,
			Func<INavigationParameters, Task<T>> asyncAction,
			Action<INavigationParameters, T> syncAction)
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
						This._thrownExceptions.OnNext(ex);
						This._asyncNavigating.OnNext(false);
						return Observable.Return<INavigationParameters>(null);
					})
					.Subscribe();
		}

		public static IObservable<INavigationParameters> WhenClosed(this ReactiveViewModel This)
		{
			return
				This.Closed
					.ObserveOn(RxApp.MainThreadScheduler);
		}

		public static IDisposable WhenClosed(
			this ReactiveViewModel This,
			Action<INavigationParameters> syncAction)
		{
			return
				This.WhenClosed()
					.Do(syncAction)
					.Catch<INavigationParameters, Exception>(ex =>
					{
						This._thrownExceptions.OnNext(ex);
						return Observable.Return<INavigationParameters>(null);
					})
					.Subscribe();
		}
	}
}