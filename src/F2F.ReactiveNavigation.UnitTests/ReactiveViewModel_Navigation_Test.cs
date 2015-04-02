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

namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveViewModel_Navigation_Test
	{
		// A test view model that can be navigated to on even milliseconds in scheduler time
 		// and that pushes a subject each time it is navigated to
		private class TestViewModel : ReactiveViewModel
		{
			private readonly Subject<Unit> _navigatedTo;
			
			public TestViewModel(Subject<Unit> navigatedTo)
			{
				_navigatedTo = navigatedTo;
			}

			internal protected override bool CanNavigateTo(INavigationParameters parameters)
			{
				return false;
			}

			internal protected override bool CanClose(INavigationParameters parameters)
			{
				return false;
			}

			internal protected override IObservable<Unit> NavigatedTo(INavigationParameters parameters)
			{
				return Observable.Start(() => _navigatedTo.OnNext(Unit.Default), RxApp.MainThreadScheduler);
			}
		}

		private readonly IFixture Fixture;

		public ReactiveViewModel_Navigation_Test()
		{
			Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
		}

		[Fact]
		public void CanNavigateTo_ShouldBeTrueByDefault()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<ReactiveViewModel>();
				sut.InitializeAsync();
				scheduler.AdvanceByMs(1);	// schedule initialization

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
				scheduler.AdvanceByMs(1);	// schedule initialization

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
				scheduler.AdvanceByMs(1);	// schedule initialization

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
				scheduler.AdvanceByMs(1);	// schedule initialization

				sut.CanClose(Fixture.Create<INavigationParameters>()).Should().BeFalse();
			});
		}

		[Fact]
		public void NavigateTo_WhenExecuteIsCalled_ShouldExecuteIrrespectiveOfCanExecute()
		{
			new TestScheduler().With(scheduler =>
			{
				var subject = new Subject<Unit>();
				int pushCount = 0;
				using (subject.Subscribe(_ => pushCount++))
				{
					Fixture.Inject(subject);

					var sut = Fixture.Create<TestViewModel>();	// this view model always returns false for CanNavigateTo
					sut.InitializeAsync();
					scheduler.AdvanceByMs(1);	// schedule initialization

					for (int i = 0; i < 10; i++)
					{
						sut.NavigateTo.Execute(null);
						scheduler.AdvanceByMs(1);	
					}

					subject.OnCompleted();
					pushCount.Should().Be(10);
				}
			});
		}


	}
}