using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
    public interface INavigationObservable<T>
    {
        ReactiveViewModel ViewModel { get; }

        IObservable<T> ToObservable();

        INavigationObservable<T> Where(Func<T, bool> predicate);

        INavigationObservable<T> Do(Action<T> syncAction);

        INavigationObservable<TResult> Do<TResult>(Func<T, TResult> syncAction);

        INavigationObservable<T> DoAsync(Func<T, Task> asyncAction);

        INavigationObservable<TResult> DoAsync<TResult>(Func<T, Task<TResult>> asyncAction);
        
        INavigationObservable<TResult> DoAsyncObservable<TResult>(Func<T, IObservable<TResult>> asyncAction);

        IDisposable Subscribe();
    }
}