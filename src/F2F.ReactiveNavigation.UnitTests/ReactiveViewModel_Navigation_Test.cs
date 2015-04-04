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
using System.Reactive.Concurrency;
using FakeItEasy;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveViewModel_Navigation_Test
	{
		// A test view model that can be navigated to on even milliseconds in scheduler time
 		// and that pushes a subject each time it is navigated to
		private class TestViewModel : ReactiveViewModel
		{		
			public TestViewModel(Subject<INavigationParameters> navigatedTo)
			{
				this.WhenNavigatedTo().Do(p => navigatedTo.OnNext(p)).Subscribe();
			}

			internal protected override bool CanNavigateTo(INavigationParameters parameters)
			{
				return false;
			}

			internal protected override bool CanClose(INavigationParameters parameters)
			{
				return false;
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
				scheduler.Advance();	// schedule initialization

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
				scheduler.Advance();	// schedule initialization

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
				scheduler.Advance();	// schedule initialization

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
				scheduler.Advance();	// schedule initialization

				sut.CanClose(Fixture.Create<INavigationParameters>()).Should().BeFalse();
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

					var sut = Fixture.Create<TestViewModel>();	// this view model always returns false for CanNavigateTo
					sut.InitializeAsync();
					scheduler.Advance();	// schedule initialization

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
		public void WhenNavigatedTo_ShouldForwardNavigationParameters()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations = sut.WhenNavigatedTo().CreateCollection();
				var parameters = Fixture.Create<INavigationParameters>();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigations.Single().Should().Be(parameters);
			});
		}

		[Fact]
		public void WhenNavigatedTo_ShouldStreamAllNavigationRequests()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations = sut.WhenNavigatedTo().CreateCollection();
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
		public void WhenNavigatedTo_ShouldNotStreamFilteredRequests()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations =
					sut
						.WhenNavigatedTo(p => p != null)
						.CreateCollection();

				var parameters = Fixture.CreateMany<INavigationParameters>(2);

				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.First());
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.Last());
				scheduler.Advance();		// schedule all previous actions

				navigations.ShouldAllBeEquivalentTo(parameters);
			});
		}

		[Fact]
		public void WhenNavigatedTo_ShouldCallSyncActionForEachFilteredRequests()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations = new List<INavigationParameters>();
				sut.WhenNavigatedTo(p => p != null, p => navigations.Add(p));

				var parameters = Fixture.CreateMany<INavigationParameters>(2);

				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.First());
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.Last());
				scheduler.Advance();		// schedule all previous actions

				navigations.ShouldAllBeEquivalentTo(parameters);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_ShouldCallAsyncActionForEachFilteredRequests()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations = new List<INavigationParameters>();
				sut.WhenNavigatedToAsync(p => p != null, p => { navigations.Add(p); return Task.FromResult(p); }, p => { });

				var parameters = Fixture.CreateMany<INavigationParameters>(2);

				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.First());
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.Last());
				scheduler.Advance();		// schedule all previous actions

				navigations.ShouldAllBeEquivalentTo(parameters);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_ShouldCallSyncActionForEachFilteredRequests()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var navigations = new List<INavigationParameters>();
				sut.WhenNavigatedToAsync(p => p != null, p => Task.FromResult(p), p => navigations.Add(p));

				var parameters = Fixture.CreateMany<INavigationParameters>(2);

				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.First());
				sut.NavigateTo(null);
				sut.NavigateTo(parameters.Last());
				scheduler.Advance();		// schedule all previous actions

				navigations.ShouldAllBeEquivalentTo(parameters);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_ShouldForwardResultOfAsyncActionToSyncAction()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var asyncResult = Fixture.Create<int>();
				var forwardedResult = 0;
				sut.WhenNavigatedToAsync(p => p != null, p => Task.FromResult(asyncResult), (p, r) => forwardedResult = r);

				var parameters = Fixture.Create<INavigationParameters>();

				sut.NavigateTo(parameters);
				scheduler.Advance();		// schedule all previous actions

				forwardedResult.Should().Be(asyncResult);
			});
		}

		// TODO: Add tests for navigation methods throwing exceptions
	}
}