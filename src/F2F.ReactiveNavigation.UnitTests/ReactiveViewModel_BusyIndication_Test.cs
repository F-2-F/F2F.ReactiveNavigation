using System;
using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using F2F.ReactiveNavigation.ViewModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Testing;
using System.Reactive.Subjects;
using Xunit;
using Microsoft.Reactive.Testing;
using FluentAssertions;
using FakeItEasy;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

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
				sut.InitializeAsync();
				scheduler.AdvanceByMs(2);	// schedule initialization

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
				var sut = A.Fake<ReactiveViewModel>();
				var navigatedToObservable =
					Observable
						.Return(Unit.Default)
						.Delay(TimeSpan.FromMilliseconds(1), scheduler);

				sut.WhenNavigatedToAsync(_ => true, _ => navigatedToObservable.ToTask(), _ => { });

				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

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

				A.CallTo(() => sut.BusyObservables()).Returns(new [] { busyObservable10Ms, busyObservable100Ms });

				var navigatedToObservable =
					Observable
						.Return(Unit.Default)
						.Delay(TimeSpan.FromMilliseconds(1), scheduler);

				sut.WhenNavigatedToAsync(_ => true, _ => navigatedToObservable.ToTask(), _ => { });
				
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				sut.NavigateTo(null);
				scheduler.Advance();	// schedule navigation call

				sut.IsBusy.Should().BeTrue();
				scheduler.AdvanceByMs(10);	// advance to pass delay of 10ms busy observable
				
				sut.IsBusy.Should().BeTrue();
				scheduler.AdvanceByMs(100);	// advance to pass delay of 100ms busy observable
				
				sut.IsBusy.Should().BeFalse();
			});
		}

		// TODO: Add tests for busy observable throwing exceptions

	}
}
