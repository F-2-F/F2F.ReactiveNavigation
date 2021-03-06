using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive;
using System.Linq.Expressions;

namespace F2F.ReactiveNavigation.ViewModel
{
    // TODO: The filter parameter is redundant to the CanNavigateTo method.
    // The CanNavigateTo method is quite handy to use in the router, the filter Func is handy in the vm implementation.
    // If we use only the filter Func, the router wouldn't know which vm could effectively be navigated to, so we would need a different mechanism
    // to communicate this. Therefore I think, we should leave the CanNavigateTo method as long as there is no better idea.
    public static class ReactiveViewModelExtensions
    {
        public static ReactiveCommand<object, Unit> CreateAsyncObservableCommand<TParam>(this ReactiveViewModel This, Action<object> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<object, Unit>(x => Observable.Start(() => executeAsync(x), sched));
        }

        public static ReactiveCommand<TParam, Unit> CreateAsyncObservableCommand<TParam>(this ReactiveViewModel This, Action<TParam> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<TParam, Unit>(x => Observable.Start(() => executeAsync(x), sched));
        }

        public static ReactiveCommand<object, TResult> CreateAsyncObservableCommand<TResult>(this ReactiveViewModel This, Func<object, TResult> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<object, TResult>(x => Observable.Start(() => executeAsync(x), sched));
        }

        public static ReactiveCommand<TParam, TResult> CreateAsyncObservableCommand<TParam, TResult>(this ReactiveViewModel This, Func<TParam, TResult> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<TParam, TResult>(x => Observable.Start(() => executeAsync(x), sched));
        }

        public static ReactiveCommand<object, Unit> CreateAsyncObservableCommand(this ReactiveViewModel This, IObservable<bool> canExecute, Action<object> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<object, Unit>(x => Observable.Start(() => executeAsync(x), sched), canExecute);
        }

        public static ReactiveCommand<TParam, Unit> CreateAsyncObservableCommand<TParam>(this ReactiveViewModel This, IObservable<bool> canExecute, Action<TParam> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<TParam, Unit>(x => Observable.Start(() => executeAsync(x), sched), canExecute);
        }

        public static ReactiveCommand<object, TResult> CreateAsyncObservableCommand<TResult>(this ReactiveViewModel This, IObservable<bool> canExecute, Func<object, TResult> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<object, TResult>(x => Observable.Start(() => executeAsync(x), sched), canExecute);
        }

        public static ReactiveCommand<TParam, TResult> CreateAsyncObservableCommand<TParam, TResult>(this ReactiveViewModel This, IObservable<bool> canExecute, Func<TParam, TResult> executeAsync, IScheduler scheduler = null)
        {
            var sched = scheduler ?? RxApp.MainThreadScheduler;

            return ReactiveCommand.CreateFromObservable<TParam, TResult>(x => Observable.Start(() => executeAsync(x), sched), canExecute);
        }

        public static IObservable<bool> WhenNull<TSender, TRet>(this TSender This, Expression<Func<TSender, TRet>> property)
            where TRet : class
        {
            return This.WhenAnyValue(property).Select(x => x == null);
        }

        public static IObservable<bool> WhenNotNull<TSender, TRet>(this TSender This, Expression<Func<TSender, TRet>> property)
            where TRet : class
        {
            return This.WhenAnyValue(property).Select(x => x != null);
        }

        public static IObservable<INavigationParameters> ObservableForNavigatedTo(this ReactiveViewModel This)
        {
            return This.NavigatedTo;
        }

        public static INavigationObservable<INavigationParameters> WhenNavigatedTo(this ReactiveViewModel This)
        {
            return new NavigationObservable<INavigationParameters>(This, This.NavigatedTo);
        }

        public static IObservable<INavigationParameters> ObservableForClosed(this ReactiveViewModel This)
        {
            return This.Closed;
        }

        public static INavigationObservable<INavigationParameters> WhenClosed(this ReactiveViewModel This)
        {
            return new NavigationObservable<INavigationParameters>(This, This.Closed);
        }

        public static IObservable<TCollectionItem> WhenAdding<TCollectionItem>(this ReactiveItemsViewModel<TCollectionItem> This)
            where TCollectionItem : class, INotifyPropertyChanged
        {
            return This.Items.BeforeItemsAdded;
        }

        public static IObservable<TCollectionItem> WhenAdded<TCollectionItem>(this ReactiveItemsViewModel<TCollectionItem> This)
            where TCollectionItem : class, INotifyPropertyChanged
        {
            return This.Items.ItemsAdded;
        }

        public static IObservable<TCollectionItem> WhenRemoved<TCollectionItem>(this ReactiveItemsViewModel<TCollectionItem> This)
            where TCollectionItem : class, INotifyPropertyChanged
        {
            return This.Items.ItemsRemoved;
        }

        public static IObservable<TCollectionItem> WhenRemoving<TCollectionItem>(this ReactiveItemsViewModel<TCollectionItem> This)
            where TCollectionItem : class, INotifyPropertyChanged
        {
            return This.Items.BeforeItemsRemoved;
        }
    }
}