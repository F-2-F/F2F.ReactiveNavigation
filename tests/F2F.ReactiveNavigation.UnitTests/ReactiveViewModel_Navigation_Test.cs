using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using F2F.Testing.Xunit.FakeItEasy;
using F2F.ReactiveNavigation.Internal;
using System.Reactive.Threading.Tasks;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class ReactiveViewModel_Navigation_Test : AutoMockFeature
    {
        // A test view model that can be navigated to on even milliseconds in scheduler time
        // and that pushes a subject each time it is navigated to
        private class TestViewModel : ReactiveViewModel
        {
            private readonly Subject<INavigationParameters> _callTracker;

            public TestViewModel(Subject<INavigationParameters> callTracker)
            {
                _callTracker = callTracker;
            }

            protected internal override async Task Initialize()
            {
                await base.Initialize();

                this.WhenNavigatedTo().Do(p => _callTracker.OnNext(p)).Subscribe();
                this.WhenClosed().Do(p => _callTracker.OnNext(p)).Subscribe();
            }

            protected internal override bool CanNavigateTo(INavigationParameters parameters)
            {
                return false;
            }

            protected internal override bool CanClose(INavigationParameters parameters)
            {
                return false;
            }
        }

        // A test view model that can be navigated to on even milliseconds in scheduler time
        // and that pushes a subject each time it is navigated to
        private class AsyncTestViewModel : ReactiveViewModel
        {
            private readonly Subject<INavigationParameters> _callTracker;
            private readonly int _navigationDelay;

            public AsyncTestViewModel(Subject<INavigationParameters> callTracker, int navigationDelay)
            {
                _callTracker = callTracker;
                _navigationDelay = navigationDelay;
            }

            protected internal override async Task Initialize()
            {
                await base.Initialize();

                this.WhenNavigatedTo()
                    .DoAsyncObservable(p => Observable.Delay(Observable.Return(p), TimeSpan.FromMilliseconds(_navigationDelay), RxApp.TaskpoolScheduler))
                    .Do(p => _callTracker.OnNext(p))
                    .Subscribe();

                this.WhenClosed().Do(p => _callTracker.OnNext(p)).Subscribe();
            }

            protected internal override bool CanNavigateTo(INavigationParameters parameters)
            {
                return false;
            }

            protected internal override bool CanClose(INavigationParameters parameters)
            {
                return false;
            }
        }

        [Fact]
        public void CanNavigateTo_ShouldBeTrueByDefault()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                sut.CanNavigateTo(Fixture.Create<INavigationParameters>()).Should().BeTrue();
            });
        }

        [Fact]
        public void CanClose_ShouldBeTrueByDefault()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                sut.CanClose(Fixture.Create<INavigationParameters>()).Should().BeTrue();
            });
        }

        [Fact]
        public void CanNavigateTo_CanBeOverriddenToReturnCustomValue()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                sut.CanNavigateTo(Fixture.Create<INavigationParameters>()).Should().BeFalse();
            });
        }

        [Fact]
        public void CanClose_CanBeOverriddenToReturnCustomValue()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                sut.CanClose(Fixture.Create<INavigationParameters>()).Should().BeFalse();
            });
        }


        [Fact]
        public void InitializeAsync_WhenThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var exception = Fixture.Create<Exception>();
                A.CallTo(() => sut.Initialize()).Throws(exception);

                var observedExceptions = sut.ThrownExceptions.CreateCollection();

                sut.InitializeAsync().Schedule(scheduler);

                observedExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void InitializeAsync_WhenThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var exception = Fixture.Create<Exception>();
                A.CallTo(() => sut.Initialize()).Throws(exception);

                sut.Invoking(_ => sut.InitializeAsync().Schedule(scheduler)).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
            });
        }

        [Fact]
        public void NavigateTo_WhenExecuteIsCalled_ShouldExecuteIrrespectiveOfCanExecute()
        {
            new TestScheduler().With(scheduler =>
            {
                var subject = new Subject<INavigationParameters>();
                int pushCount = 0;
                using (subject.Subscribe(_ => pushCount++))
                {
                    Fixture.Inject(subject);

                    var sut = Fixture.Create<TestViewModel>();    // this view model always returns false for CanNavigateTo
                    sut.InitializeAsync();
                    scheduler.Advance();    // schedule initialization

                    for (int i = 0; i < 10; i++)
                    {
                        sut.NavigateTo(null);
                        scheduler.Advance();
                    }

                    subject.OnCompleted();
                    pushCount.Should().Be(10);
                }
            });
        }

        [Fact]
        public void Close_WhenExecuteIsCalled_ShouldExecuteIrrespectiveOfCanExecute()
        {
            new TestScheduler().With(scheduler =>
            {
                var subject = new Subject<INavigationParameters>();
                int pushCount = 0;
                using (subject.Subscribe(_ => pushCount++))
                {
                    Fixture.Inject(subject);

                    var sut = Fixture.Create<TestViewModel>();    // this view model always returns false for CanClose
                    sut.InitializeAsync();
                    scheduler.Advance();    // schedule initialization

                    for (int i = 0; i < 10; i++)
                    {
                        sut.Close(null);
                        scheduler.Advance();
                    }

                    subject.OnCompleted();
                    pushCount.Should().Be(10);
                }
            });
        }

        [Fact]
        public void NavigateTo_ShouldForwardNavigationParameters()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = sut.ObservableForNavigatedTo().CreateCollection();
                var parameters = Fixture.Create<INavigationParameters>();

                sut.NavigateTo(parameters);
                scheduler.Advance();

                navigations.Single().Should().Be(parameters);
            });
        }

        [Fact]
        public void NavigateTo_ShouldStreamAllNavigationRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = sut.ObservableForNavigatedTo().CreateCollection();
                var parameters = Fixture.CreateMany<INavigationParameters>(Fixture.Create<int>());

                foreach (var p in parameters)
                {
                    sut.NavigateTo(p);
                    scheduler.Advance();
                }

                navigations.ShouldAllBeEquivalentTo(parameters);
            });
        }

        [Fact]
        public void NavigateTo_ShouldNotStreamFilteredRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = sut.WhenNavigatedTo().Where(p => p != null).ToObservable().CreateCollection();

                var parameters = Fixture.CreateMany<INavigationParameters>(2);

                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.First());
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.Last());
                scheduler.Advance();        // schedule all previous actions

                navigations.ShouldAllBeEquivalentTo(parameters);
            });
        }
        
        [Fact]
        public void NavigateTo_ShouldCallSyncActionForEachFilteredRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = new List<INavigationParameters>();
                sut.WhenNavigatedTo()
                    .Where(p => p != null)
                    .Do(p => navigations.Add(p))
                    .Subscribe();

                var parameters = Fixture.CreateMany<INavigationParameters>(2);

                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.First());
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.Last());
                scheduler.Advance();        // schedule all previous actions

                navigations.ShouldAllBeEquivalentTo(parameters);
            });
        }

        [Fact]
        public void NavigateTo_ShouldCallAsyncActionForEachFilteredRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = new List<INavigationParameters>();
                sut.WhenNavigatedTo()
                    .Where(p => p != null)
                    .DoAsync(p => { navigations.Add(p); return Task.FromResult(p); })
                    .Subscribe();

                var parameters = Fixture.CreateMany<INavigationParameters>(2);

                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.First());
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.Last());
                scheduler.Advance();        // schedule all previous actions

                navigations.ShouldAllBeEquivalentTo(parameters);
            });
        }

        [Fact]
        public void __WhenNavigatedTo_ShouldCallSyncActionForEachFilteredRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var navigations = new List<INavigationParameters>();
                sut.WhenNavigatedTo()
                    .Where(p => p != null)
                    .DoAsync(p => Task.FromResult(p))
                    .Do(p => navigations.Add(p))
                    .Subscribe();

                var parameters = Fixture.CreateMany<INavigationParameters>(2);

                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.First());
                sut.NavigateTo(null);
                sut.NavigateTo(parameters.Last());
                scheduler.Advance();        // schedule all previous actions

                navigations.ShouldAllBeEquivalentTo(parameters);
            });
        }

        [Fact]
        public void WhenNavigatedTo_ShouldForwardResultOfAsyncActionToSyncAction()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var asyncResult = Fixture.Create<int>();
                var forwardedResult = 0;
                sut.WhenNavigatedTo()
                    .Where(p => p != null)
                    .DoAsync(p => Task.FromResult(asyncResult))
                    .Do(r => forwardedResult = r)
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();

                sut.NavigateTo(parameters);
                scheduler.Advance();        // schedule all previous actions

                forwardedResult.Should().Be(asyncResult);
            });
        }

        [Fact]
        public async Task __WhenNavigatedTo_WhenSyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            var exception = Fixture.Create<Exception>();
            sut.WhenNavigatedTo()
                .Do(_ => { throw exception; })
                .Subscribe();

            var parameters = Fixture.Create<INavigationParameters>();

            sut.Invoking(x => x.NavigateTo(parameters)).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
        }

        [Fact]
        public void WhenNavigatedTo_WhenSyncActionThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .Do(_ => { throw exception; })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();
                var navigationExceptions = sut.ThrownExceptions.CreateCollection();

                sut.NavigateTo(parameters);
                scheduler.Advance();

                navigationExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenFilterThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .Where(_ => { throw exception; })
                    .DoAsync(p => Task.FromResult(p))
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();

                Action<TestViewModel> action = x =>
                {
                    x.NavigateTo(parameters);
                    scheduler.Advance();
                };

                sut.Invoking(action).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenFilterThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .Where(_ => { throw exception; })
                    .DoAsync(p => Task.FromResult(p))
                    .Do(_ => { })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();
                var navigationExceptions = sut.ThrownExceptions.CreateCollection();

                sut.NavigateTo(parameters);
                scheduler.Advance();

                navigationExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenAsyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .DoAsync(_ => { throw exception; })
                    .Do(_ => { })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();

                Action<TestViewModel> action = x =>
                {
                    x.NavigateTo(parameters);
                    scheduler.Advance();
                };

                sut.Invoking(action).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenAsyncActionThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .DoAsync(_ => { throw exception; })
                    .Do(_ => { })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();
                var navigationExceptions = sut.ThrownExceptions.CreateCollection();

                sut.NavigateTo(parameters);
                scheduler.Advance();

                navigationExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenSyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .Do(_ => { throw exception; })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();

                Action<TestViewModel> action = x =>
                {
                    x.NavigateTo(parameters);
                    scheduler.Advance();
                };

                sut.Invoking(action).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenAsyncSelectorActionPerformsLongRunningOperation_ShouldMoep()
        {
            new TestScheduler().With(scheduler =>
            {
                var subject = new Subject<INavigationParameters>();
                int pushCount = 0;
                using (subject.Subscribe(_ => pushCount++))
                {
                    var sut = new AsyncTestViewModel(subject, 1000);
                    sut.InitializeAsync();
                    scheduler.Advance();    // schedule initialization

                    var parameters = Fixture.Create<INavigationParameters>();

                    sut.NavigateTo(parameters);
                    scheduler.Advance();
                    pushCount.Should().Be(0);

                    scheduler.AdvanceByMs(999);
                    pushCount.Should().Be(0);

                    scheduler.AdvanceByMs(1);
                    pushCount.Should().Be(1);
                }
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenAsyncSelectorActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .DoAsync<object>(_ => { throw exception; })
                    .Do(_ => { })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();

                Action<TestViewModel> action = x =>
                {
                    x.NavigateTo(parameters);
                    scheduler.Advance();
                };

                sut.Invoking(action).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
            });
        }

        [Fact]
        public void WhenNavigatedTo_WhenAsyncSelectorActionThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var exception = Fixture.Create<Exception>();
                sut.WhenNavigatedTo()
                    .DoAsync<object>(_ => { throw exception; })
                    .Do(_ => { })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();
                var navigationExceptions = sut.ThrownExceptions.CreateCollection();

                sut.NavigateTo(parameters);
                scheduler.Advance();

                navigationExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void WhenClosed_ShouldForwardNavigationParameters()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var closeRequests = sut.ObservableForClosed().CreateCollection();
                var parameters = Fixture.Create<INavigationParameters>();

                sut.Close(parameters);
                scheduler.Advance();

                closeRequests.Single().Should().Be(parameters);
            });
        }

        [Fact]
        public void WhenClosed_ShouldStreamAllCloseRequests()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var closeRequests = sut.ObservableForClosed().CreateCollection();
                var parameters = Fixture.CreateMany<INavigationParameters>(Fixture.Create<int>());

                foreach (var p in parameters)
                {
                    sut.Close(p);
                    scheduler.Advance();
                }

                closeRequests.ShouldAllBeEquivalentTo(parameters);
            });
        }

        [Fact]
        public async Task WhenClosed_WhenSyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            var exception = Fixture.Create<Exception>();
            sut.WhenClosed()
                .Do(_ => { throw exception; })
                .Subscribe();

            var parameters = Fixture.Create<INavigationParameters>();

            // intentionally don't observe the thrown exceptions!
            //var exceptions = sut.ThrownExceptions.CreateCollection();

            sut.Invoking(x => x.Close(parameters)).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
        }

        [Fact]
        public void WhenClosed_WhenSyncActionThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<TestViewModel>();
                sut.InitializeAsync();
                scheduler.Advance();    // schedule initialization

                var exception = Fixture.Create<Exception>();
                sut.WhenClosed()
                    .Do(_ => { throw exception; })
                    .Subscribe();

                var parameters = Fixture.Create<INavigationParameters>();
                var exceptions = sut.ThrownExceptions.CreateCollection();

                sut.Close(parameters);
                scheduler.Advance();

                exceptions.Single().Should().Be(exception);
            });
        }
    }
}