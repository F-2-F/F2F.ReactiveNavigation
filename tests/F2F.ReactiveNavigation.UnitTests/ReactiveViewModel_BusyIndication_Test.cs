using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using F2F.Testing.Xunit.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class ReactiveViewModel_BusyIndication_Test : AutoMockFeature
    {
        [Fact]
        public void IsBusy_ShouldBeFalseByDefault()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();
                sut.InitializeAsync().Schedule(scheduler);

                sut.IsBusy.Should().BeFalse();
            });
        }

        [Fact]
        public void IsBusy_ShouldBeTrueWhenNotYetInitialized()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();

                sut.IsBusy.Should().BeTrue();
            });
        }

        [Fact]
        public void Task_ShouldBeTrueWhenNavigateToAsyncIsExecuting()
        {
            new TestScheduler().With(scheduler =>
            {
                var navigatedToObservable =
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromMilliseconds(100), scheduler);

                var task = navigatedToObservable.ToTask();

                task.IsCompleted.Should().BeFalse();

                scheduler.AdvanceByMs(101);

                task.IsCompleted.Should().BeTrue();
            });
        }

        [Fact(Skip = "Rethink this test")]
        public void IsBusy_ShouldBeTrueWhenNavigateToAsyncIsExecuting()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();
                var navigatedToObservable =
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromMilliseconds(1), scheduler);

                sut.WhenNavigatedTo()
                    .DoAsync(_ => navigatedToObservable.ToTask() as Task)
                    .Subscribe();

                sut.InitializeAsync().Schedule(scheduler);

                for (int i = 0; i < 10; i++)
                {
                    sut.NavigateTo(null);
                    scheduler.Advance();    // schedule navigation call

                    sut.IsBusy.Should().BeTrue();
                    scheduler.Advance();    // pass delay in navigatedToObservable

                    sut.IsBusy.Should().BeFalse();
                }
            });
        }

        [Fact(Skip = "Rethink this test")]
        public void IsBusy_ShouldBeTrueWhenNavigateToAsyncWithResultIsExecuting()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = Fixture.Create<ReactiveViewModel>();
                var navigatedToObservable =
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromMilliseconds(1), scheduler);

                sut.WhenNavigatedTo()
                    .DoAsync(_ => navigatedToObservable.ToTask())
                    .Subscribe();

                sut.InitializeAsync().Schedule(scheduler);

                for (int i = 0; i < 10; i++)
                {
                    sut.NavigateTo(null);
                    scheduler.Advance();    // schedule navigation call

                    sut.IsBusy.Should().BeTrue();
                    scheduler.Advance();    // pass delay in navigatedToObservable

                    sut.IsBusy.Should().BeFalse();
                }
            });
        }

        [Fact]
        public void IsBusy_WhenHavingOneBusyObservable_ShouldBeTrueAsLongAsBusyObservableYieldsTrue()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var busySubject = new Subject<bool>();

                A.CallTo(() => sut.BusyObservables).Returns(new[] { busySubject });

                sut.IsBusy.Should().BeTrue();

                sut.InitializeAsync().Schedule(scheduler);
                sut.IsBusy.Should().BeFalse();

                busySubject.OnNext(true);
                sut.IsBusy.Should().BeTrue();

                busySubject.OnNext(false);
                sut.IsBusy.Should().BeFalse();
            });
        }

        [Fact]
        public void IsBusy_WhenHavingTwoBusyObservables_ShouldBeTrueAsLongAsOneBusyObservableYieldsTrue()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var busySubject1 = new Subject<bool>();
                var busySubject2 = new Subject<bool>();

                A.CallTo(() => sut.BusyObservables).Returns(new[] { busySubject1, busySubject2 });

                sut.IsBusy.Should().BeTrue();

                sut.InitializeAsync().Schedule(scheduler);
                sut.IsBusy.Should().BeFalse();

                busySubject1.OnNext(true);
                sut.IsBusy.Should().BeTrue();

                busySubject2.OnNext(true);
                sut.IsBusy.Should().BeTrue();

                busySubject1.OnNext(false);
                sut.IsBusy.Should().BeTrue();

                busySubject2.OnNext(false);
                sut.IsBusy.Should().BeFalse();
            });
        }

        [Fact(Skip = "Rethink this test")]
        public void IsBusy_WhenHavingTwoBusyObservables_AndNavigation_ShouldBeTrueAsLongAsOneBusyObservableYieldsTrue()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var busySubject1 = new Subject<bool>();
                var busySubject2 = new Subject<bool>();

                A.CallTo(() => sut.BusyObservables).Returns(new[] { busySubject1, busySubject2 });

                var navigatedToTask =
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromMilliseconds(2), scheduler)
                        .ToTask();

                sut.IsBusy.Should().BeTrue();

                sut.WhenNavigatedTo()
                    .DoAsync(_ => navigatedToTask)
                    .Subscribe();

                sut.InitializeAsync().Schedule(scheduler);
                sut.IsBusy.Should().BeFalse();

                sut.NavigateTo(NavigationParameters.Empty);
                sut.IsBusy.Should().BeFalse();

                scheduler.Advance();    // schedule navigation call start
                sut.IsBusy.Should().BeTrue();

                scheduler.Advance();
                sut.IsBusy.Should().BeFalse();    // schedule navigation call end

                busySubject2.OnNext(true);
                sut.IsBusy.Should().BeTrue();

                busySubject2.OnNext(false);
                sut.IsBusy.Should().BeFalse();
            });
        }

        [Fact]
        public void IsBusy_WhenBusyObservableThrowsObservedException_ShouldPushExceptionToThrownExceptionsObservable()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var exception = Fixture.Create<Exception>();
                var errorSubject = new Subject<bool>();

                A.CallTo(() => sut.BusyObservables).Returns(new[] { errorSubject });
                sut.InitializeAsync();

                var busyExceptions = sut.ThrownExceptions.CreateCollection();

                errorSubject.OnError(exception);
                scheduler.Advance();

                busyExceptions.Single().Should().Be(exception);
            });
        }

        [Fact]
        public void IsBusy_WhenBusyObservableThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
        {
            new TestScheduler().With(scheduler =>
            {
                var sut = A.Fake<ReactiveViewModel>();
                var exception = Fixture.Create<Exception>();
                var errorSubject = new Subject<bool>();

                A.CallTo(() => sut.BusyObservables).Returns(new[] { errorSubject });
                sut.InitializeAsync();

                errorSubject.OnError(exception);

                scheduler
                    .Invoking(x => x.Advance())
                    .ShouldThrow<Exception>()
                    .Which
                    .InnerException
                    .Should()
                    .Be(exception);
            });
        }
    }
}