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

namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveViewModel_Navigation_Test
	{
		// A test view model that can be navigated to on even milliseconds in scheduler time
		// and that pushes a subject each time it is navigated to
		private class TestViewModel : ReactiveViewModel
		{
			public TestViewModel(Subject<F2F.ReactiveNavigation.ViewModel.INavigationParameters> navigatedTo)
			{
				this.WhenNavigatedTo().Do(p => navigatedTo.OnNext(p)).Subscribe();
			}

			protected internal override bool CanNavigateTo(F2F.ReactiveNavigation.ViewModel.INavigationParameters parameters)
			{
				return false;
			}

			protected internal override bool CanClose(F2F.ReactiveNavigation.ViewModel.INavigationParameters parameters)
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

				sut.CanNavigateTo(Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>()).Should().BeTrue();
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

				sut.CanClose(Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>()).Should().BeTrue();
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

				sut.CanNavigateTo(Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>()).Should().BeFalse();
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

				sut.CanClose(Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>()).Should().BeFalse();
			});
		}

		[Fact]
		public void NavigateTo_WhenExecuteIsCalled_ShouldExecuteIrrespectiveOfCanExecute()
		{
			new TestScheduler().With(scheduler =>
			{
				var subject = new Subject<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
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
				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

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
				var parameters = Fixture.CreateMany<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(Fixture.Create<int>());

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

				var parameters = Fixture.CreateMany<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(2);

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

				var navigations = new List<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				sut.WhenNavigatedTo(p => p != null, p => navigations.Add(p));

				var parameters = Fixture.CreateMany<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(2);

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

				var navigations = new List<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				sut.WhenNavigatedToAsync(p => p != null, p => { navigations.Add(p); return Task.FromResult(p); }, p => { });

				var parameters = Fixture.CreateMany<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(2);

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

				var navigations = new List<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				sut.WhenNavigatedToAsync(p => p != null, p => Task.FromResult(p), p => navigations.Add(p));

				var parameters = Fixture.CreateMany<F2F.ReactiveNavigation.ViewModel.INavigationParameters>(2);

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

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

				sut.NavigateTo(parameters);
				scheduler.Advance();		// schedule all previous actions

				forwardedResult.Should().Be(asyncResult);
			});
		}

		[Fact]
		public async Task WhenNavigatedTo_WhenFilterThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			var sut = Fixture.Create<TestViewModel>();
			await sut.InitializeAsync();

			var exception = Fixture.Create<Exception>();
			sut.WhenNavigatedTo(_ => { throw exception; }, _ => { });

			var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

			// intentionally don't observe the navigation exceptions!
			//var navigationExceptions = sut.ThrownExceptions.CreateCollection();

			sut.Invoking(x => x.NavigateTo(parameters)).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
		}

		[Fact]
		public void WhenNavigatedTo_WhenFilterThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedTo(_ => { throw exception; }, _ => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}

		[Fact]
		public async Task WhenNavigatedTo_WhenSyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			var sut = Fixture.Create<TestViewModel>();
			await sut.InitializeAsync();

			var exception = Fixture.Create<Exception>();
			sut.WhenNavigatedTo(_ => true, _ => { throw exception; });

			var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

			// intentionally don't observe the navigation exceptions!
			//var navigationExceptions = sut.ThrownExceptions.CreateCollection();

			sut.Invoking(x => x.NavigateTo(parameters)).ShouldThrow<Exception>().Which.InnerException.Should().Be(exception);
		}

		[Fact]
		public void WhenNavigatedTo_WhenSyncActionThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedTo(_ => true, _ => { throw exception; });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenFilterThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => { throw exception; }, p => Task.FromResult(p), _ => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

				// intentionally don't observe the navigation exceptions!
				//var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut
					.Invoking(x =>
					{
						x.NavigateTo(parameters);
						scheduler.Advance();
					})
					.ShouldThrow<Exception>()
					.Which
					.InnerException
					.Should()
					.Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenFilterThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => { throw exception; }, p => Task.FromResult(p), _ => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenAsyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => true, _ => { throw exception; }, _ => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

				// intentionally don't observe the navigation exceptions!
				//var navigationExceptions = sut.ThrownNavigationExceptions.CreateCollection();

				sut
					.Invoking(x =>
					{
						x.NavigateTo(parameters);
						scheduler.Advance();
					})
					.ShouldThrow<Exception>()
					.Which
					.InnerException
					.Should()
					.Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenAsyncActionThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => true, _ => { throw exception; }, _ => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenSyncActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => true, p => Task.FromResult(p), _ => { throw exception; });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

				// intentionally don't observe the navigation exceptions!
				//var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut
					.Invoking(x =>
					{
						x.NavigateTo(parameters);
						scheduler.Advance();
					})
					.ShouldThrow<Exception>()
					.Which
					.InnerException
					.Should()
					.Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenSyncActionThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync(_ => true, p => Task.FromResult(p), _ => { throw exception; });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenAsyncSelectorActionThrowsUnobservedException_ShouldThrowDefaultExceptionAtCallSite()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync<object>(_ => true, _ => { throw exception; }, (p, r) => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();

				// intentionally don't observe the navigation exceptions!
				//var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut
					.Invoking(x =>
					{
						x.NavigateTo(parameters);
						scheduler.Advance();
					})
					.ShouldThrow<Exception>()
					.Which
					.InnerException
					.Should()
					.Be(exception);
			});
		}

		[Fact]
		public void WhenNavigatedToAsync_WhenAsyncSelectorActionThrowsObservedException_ShouldPushExceptionToThrownNavigationExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				var exception = Fixture.Create<Exception>();
				sut.WhenNavigatedToAsync<object>(_ => true, _ => { throw exception; }, (p, r) => { });

				var parameters = Fixture.Create<F2F.ReactiveNavigation.ViewModel.INavigationParameters>();
				var navigationExceptions = sut.ThrownExceptions.CreateCollection();

				sut.NavigateTo(parameters);
				scheduler.Advance();

				navigationExceptions.Single().Should().Be(exception);
			});
		}
	}
}