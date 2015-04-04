using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public static class ReactiveViewModelExtensions
	{
		internal static IObservable<INavigationParameters> WhenNavigatedTo(this ReactiveViewModel This)
		{
			return This._navigateTo.ObserveOn(RxApp.MainThreadScheduler);
		}

		internal static IObservable<INavigationParameters> WhenNavigatedTo(
			this ReactiveViewModel This,
			Func<INavigationParameters, bool> filter)
		{
			return This.WhenNavigatedTo().Where(filter);
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
							This._thrownNavigationExceptions.OnNext(ex);
							return Observable.Return<INavigationParameters>(null);
						})
					.Subscribe();
		}

		internal static IObservable<INavigationParameters> WhenNavigatedToAsync(this ReactiveViewModel This)
		{
			return This._navigateTo.ObserveOn(RxApp.TaskpoolScheduler);
		}

		internal static IObservable<INavigationParameters> WhenNavigatedToAsync(this ReactiveViewModel This, Func<INavigationParameters, bool> filter)
		{
			return This.WhenNavigatedToAsync().Where(filter);
		}

		public static IDisposable WhenNavigatedToAsync(
			this ReactiveViewModel This, 
			Func<INavigationParameters, bool> filter,
			Func<INavigationParameters, Task> asyncAction,
			Action<INavigationParameters> syncAction)
		{
			return
				This.WhenNavigatedToAsync(filter)
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
						This._thrownNavigationExceptions.OnNext(ex);
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
				This.WhenNavigatedToAsync(filter)
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
						return Observable.Return<INavigationParameters>(null);
					})
					.Subscribe();
		}

	}
}
