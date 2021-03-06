﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
    public class ReactiveViewModel : ReactiveObject, IHaveTitle, ISupportBusyIndication
    {
        private interface INavigationCall
        {
            INavigationParameters Parameters { get; }
        }

        private class NavigateToCall : INavigationCall
        {
            public INavigationParameters Parameters { get; set; }
        }

        private class CloseCall : INavigationCall
        {
            public INavigationParameters Parameters { get; set; }
        }

        private string _title;
        private bool _isBusy = true;
        private Task _initializationTask;

        private readonly Subject<INavigationCall> _navigation = new Subject<INavigationCall>();
        private readonly ISubject<bool, bool> _asyncNavigating = Subject.Synchronize(new Subject<bool>());
        private readonly ScheduledSubject<Exception> _thrownExceptions;

        private readonly IObserver<Exception> _defaultExceptionHandler =
            Observer.Create<Exception>(ex =>
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    throw new Exception(
                        "An OnError occurred on an ReactiveViewModel, that would break the observables. To prevent this, Subscribe to the ThrownExceptions property of your objects",
                        ex);
                });
            });

        public ReactiveViewModel()
        {
            _thrownExceptions = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, _defaultExceptionHandler);
        }

        public async Task InitializeAsync()
        {
            // prevent from initializing more than once
            
            // TODO: This is not thread-safe. 
            if (_initializationTask == null)
            {
                _initializationTask = InitializeAsyncCore();
            }

            await _initializationTask;
        }

        private async Task InitializeAsyncCore()
        {
            try
            {
                IsBusy = true;
                _asyncNavigating.OnNext(true);

                await Initialize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _thrownExceptions.OnNext(ex);
            }
            finally
            {
                _asyncNavigating.OnNext(false);
                IsBusy = false;
            }

            // configure BusyObservables after Initialize call, since sub-classes may create instances that contribute to IsBusy
            // during initialization. 
            await Observable.Start(() =>
            {
                BusyObservables
                    .Select(o => o.StartWith(false))
                    .Concat(new[] { _asyncNavigating.StartWith(false) })
                    .Select(o => o.StartWith(false))
                    .CombineLatest()
                    .Select(bs => bs.Any(b => b))
                    .Do(b => IsBusy = b)
                    .Catch<bool, Exception>(ex =>
                    {
                        _thrownExceptions.OnNext(ex);
                        return Observable.Return(false);
                    })
                    .Subscribe();

            }, RxApp.MainThreadScheduler).ToTask();
        }

        internal IObservable<INavigationParameters> NavigatedTo
        {
            get
            {
                return _navigation
                    .OfType<ReactiveViewModel.NavigateToCall>()
                    .Select(c => c.Parameters);
            }
        }

        internal IObservable<INavigationParameters> Closed
        {
            get
            {
                return _navigation
                    .OfType<ReactiveViewModel.CloseCall>()
                    .Select(c => c.Parameters);
            }
        }

        internal ISubject<bool, bool> AsyncNavigatingSource
        {
            get { return _asyncNavigating; }
        }

        internal ScheduledSubject<Exception> ThrownExceptionsSource
        {
            get { return _thrownExceptions; }
        }

        public new IObservable<Exception> ThrownExceptions
        {
            get { return _thrownExceptions.Merge(base.ThrownExceptions); }
        }

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        public bool IsInitialized
        {
            get { return _initializationTask != null && _initializationTask.IsCompleted; }
        }

        protected internal virtual IEnumerable<IObservable<bool>> BusyObservables
        {
            get { yield return Observable.Return(false); }
        }

        protected internal virtual Task Initialize()
        {
            return Task.FromResult(false);
        }

        protected internal virtual bool CanNavigateTo(INavigationParameters parameters)
        {
            return true;
        }

        internal void NavigateTo(INavigationParameters parameters)
        {
            _navigation.OnNext(new NavigateToCall() { Parameters = parameters });
        }

        // implemented synchronously, since CanClose should only ever ask the user, if she is ok with closing.
        protected internal virtual bool CanClose(INavigationParameters parameters)
        {
            return true;
        }

        internal void Close(INavigationParameters parameters)
        {
            _navigation.OnNext(new CloseCall() { Parameters = parameters });
        }
    }
}