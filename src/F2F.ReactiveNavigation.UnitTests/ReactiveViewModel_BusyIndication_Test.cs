using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
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

namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveViewModel_BusyIndication_Test
	{
		private readonly IFixture Fixture;

		public ReactiveViewModel_BusyIndication_Test()
		{
			Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
		}

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
		public void IsBusy_ShouldBeTrueWhenNavigateToAsyncIsExecuting()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<ReactiveViewModel>();
				var navigatedToObservable =
					Observable
						.Return(Unit.Default)
						.Delay(TimeSpan.FromMilliseconds(1), scheduler);

				sut.WhenNavigatedToAsync(_ => true, _ => navigatedToObservable.ToTask(), _ => { });

				sut.InitializeAsync().Schedule(scheduler);

				for (int i = 0; i < 10; i++)
				{
					sut.NavigateTo(null);
					scheduler.Advance();	// schedule navigation call

					sut.IsBusy.Should().BeTrue();
					scheduler.Advance();	// pass delay in navigation observable

					sut.IsBusy.Should().BeFalse();
				}
			});
		}

		[Fact]
		public void IsBusy_ShouldBeTrueAsLongAsBusyObservableYieldsTrue()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = A.Fake<ReactiveViewModel>();
				var busyObservable10Ms =
					Observable
						.Return(false)
						.Delay(TimeSpan.FromMilliseconds(10), scheduler)
						.StartWith(true);

				var busyObservable100Ms =
					Observable
						.Return(false)
						.Delay(TimeSpan.FromMilliseconds(100), scheduler)
						.StartWith(true);

				A.CallTo(() => sut.BusyObservables).Returns(new[] { busyObservable10Ms, busyObservable100Ms });

				var navigatedToObservable =
					Observable
						.Return(Unit.Default)
						.Delay(TimeSpan.FromMilliseconds(1), scheduler);

				sut.WhenNavigatedToAsync(_ => true, _ => navigatedToObservable.ToTask(), _ => { });

				sut.InitializeAsync().Schedule(scheduler);

				sut.NavigateTo(null);
				scheduler.Advance();	// schedule navigation call

				sut.IsBusy.Should().BeTrue();
				scheduler.AdvanceByMs(10);	// advance to pass delay of 10ms busy observable

				sut.IsBusy.Should().BeTrue();
				scheduler.AdvanceByMs(100);	// advance to pass delay of 100ms busy observable

				sut.IsBusy.Should().BeFalse();
			});
		}

		[Fact]
		public void IsBusy_WhenBusyObservableThrowsObservedException_ShouldPushExceptionToThrownBusyExceptionsObservable()
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