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
}