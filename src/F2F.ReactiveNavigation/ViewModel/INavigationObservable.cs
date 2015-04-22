using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public interface INavigationObservable<T>
		where T : class
	{
		ReactiveViewModel ViewModel { get; }

		INavigationObservable<T> Where(Func<T, bool> predicate);

		INavigationObservable<T> Do(Action<T> syncAction);

		INavigationObservable<T> DoAsync(Func<T, Task> asyncAction);

		IDisposable Subscribe();
	}
}