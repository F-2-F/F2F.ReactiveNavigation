using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using F2F.Testing.Xunit.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using Ploeh.AutoFixture.Idioms;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class Router_Test : AutoMockFeature
	{
		[Fact]
		public void AssertProperNullGuards()
		{
			var assertion = new GuardClauseAssertion(Fixture);
			assertion.Verify(typeof(Internal.Router));
		}

		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsNoViewModel_ShouldCreateNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = Fixture.Create<ReactiveViewModel>();

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened();
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsNoViewModel_ShouldNavigateToNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = Fixture.Create<ReactiveViewModel>();

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);
				scheduler.AdvanceByMs(100);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsNoViewModel_ShouldNotCallCanNavigateToOnNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = A.Fake<ReactiveViewModel>();

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustNotHaveHappened();
			});
		}
		
		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsNoViewModel_ShouldCallAddOnRegion()
		{
			new TestScheduler().With(scheduler =>
			{
				var parameters = Fixture.Create<INavigationParameters>();
				var region = Fixture.Create<Internal.IRegion>();

				var sut = Fixture.Create<Internal.Router>();
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters);
				scheduler.Advance();

				A.CallTo(() => region.Add<ReactiveViewModel>()).MustHaveHappened();
			});
		}
		
		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsViewModel_AndViewModelCannotBeNavigatedTo_ShouldCreateNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(false);

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened(Repeated.Exactly.Twice);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsViewModel_AndViewModelCanBeNavigatedTo_ShouldCallCanNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenRegionContainsViewModel_AndViewModelCanBeNavigatedTo_ShouldNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				Fixture.Inject(viewModelFactory);
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForNavigatedTo().CreateCollection();

				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(2);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenViewModelCanBeNavigatedTo_ShouldCallCanNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(viewModel)).Returns(true);

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigateAsync(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenViewModelCanBeNavigatedTo_ShouldNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(viewModel)).Returns(true);

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigateAsync(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenViewModelCannotBeNavigatedTo_ShouldCallCanNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(false);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(viewModel)).Returns(true);

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigateAsync(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigateAsync_WhenViewModelCannotBeNavigatedTo_ShouldNotNavigateTo()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(false);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(viewModel)).Returns(true);

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigateAsync(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(0);
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsViewModel_ShouldCallCanClose()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Find(A<Func<ReactiveViewModel, bool>>.Ignored))
					.Invokes(c => ((Func<ReactiveViewModel, bool>)c.Arguments[0])(viewModel))
					.Returns(new[] { viewModel });

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanClose(parameters)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsViewModel_ShouldCallRemoveOnRegion()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Find(A<Func<ReactiveViewModel, bool>>.Ignored))
					.Invokes(c => ((Func<ReactiveViewModel, bool>)c.Arguments[0])(viewModel))
					.Returns(new[] { viewModel });

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => region.Remove(viewModel)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsViewModel_AndViewModelCanBeClosed_ShouldClose()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(true);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Find(A<Func<ReactiveViewModel, bool>>.Ignored))
					.Invokes(c => ((Func<ReactiveViewModel, bool>)c.Arguments[0])(viewModel))
					.Returns(new[] { viewModel });

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForClosed().CreateCollection();

				// Act
				sut.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsViewModel_AndViewModelCannotBeClosed_ShouldNotClose()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(false);

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Find(A<Func<ReactiveViewModel, bool>>.Ignored))
					.Invokes(c => ((Func<ReactiveViewModel, bool>)c.Arguments[0])(viewModel))
					.Returns(Enumerable.Empty<ReactiveViewModel>());

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.ObservableForClosed().CreateCollection();

				// Act
				sut.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(0);
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsNoViewModel_ShouldNotThrow()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModel = A.Fake<ReactiveViewModel>();

				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject<IScheduler>(scheduler);
				var sut = Fixture.Create<Internal.Router>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Find(A<Func<ReactiveViewModel, bool>>.Ignored)).Returns(Enumerable.Empty<ReactiveViewModel>());

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.Invoking(x => x.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler)).ShouldNotThrow();
			});
		}

		[Fact]
		public void RequestCloseAsync_WhenRegionContainsViewModel_ShouldEndLifetimeUsingScope()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				var scope = Fixture.Create<IDisposable>();

				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.Lifetime().EndingWith(scope));
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestCloseAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				A.CallTo(() => scope.Dispose()).MustHaveHappened();
			});
		}

		[Fact]
		public async Task RequestNavigateAsync_WhenCanNavigateToThrowsException_ShouldThrowExceptionAtOriginOfNavigationRequest()
		{
			// Arrange
			var viewModelFactory = Fixture.Create<ICreateViewModel>();
			var viewModel = A.Fake<ReactiveViewModel>();
			A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

			var exception = Fixture.Create<Exception>();
			A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Throws(exception);

			Fixture.Inject(viewModelFactory);

			var sut = Fixture.Create<Internal.Router>();
			var region = Fixture.Create<Internal.Region>();

			var parameters = Fixture.Create<INavigationParameters>();
			// the first navigation request creates a new view model and therefore doesn't call CanNavigateTo
			await sut.RequestNavigateAsync<ReactiveViewModel>(region, parameters);

			// Act
			sut
				.Awaiting(x => x.RequestNavigateAsync<ReactiveViewModel>(region, parameters))
				.ShouldThrow<Exception>()
				.Which
				.Should()
				.Be(exception);
		}

		[Fact]
		public void RequestNavigateAsync_WhenNavigatedToThrowsUnobservedException_ShouldThrowExceptionAtOriginOfNavigationRequest()
		{
			// Arrange
			var viewModelFactory = Fixture.Create<ICreateViewModel>();
			var viewModel = A.Fake<ReactiveViewModel>();
			A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

			var exception = Fixture.Create<Exception>();
			viewModel.WhenNavigatedTo()
				.Where(_ => true)
				.Do(_ => { throw exception; })
				.Subscribe();

			Fixture.Inject(viewModelFactory);

			var sut = Fixture.Create<Internal.Router>();
			var region = Fixture.Create<Internal.Region>();

			var parameters = Fixture.Create<INavigationParameters>();

			// Act
			sut
				.Awaiting(x => x.RequestNavigateAsync<ReactiveViewModel>(region, parameters))
				.ShouldThrow<Exception>()
				.Which
				.InnerException		// unobserved navigation exceptions are handled by a default handler that wraps the original exception
				.Should()
				.Be(exception);
		}

		[Fact]
		public void RequestNavigateAsync_WhenInitializeThrowsUnobservedException_ShouldThrowExceptionAtOrigin()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

				var exception = Fixture.Create<Exception>();
				A.CallTo(() => viewModel.Initialize()).Throws(exception);

				Fixture.Inject(viewModelFactory);

				var sut = Fixture.Create<Internal.Router>();
				var region = Fixture.Create<Internal.Region>();

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut
					.Awaiting(x => x.RequestNavigateAsync<ReactiveViewModel>(region, parameters).Schedule(scheduler))
					.ShouldThrow<Exception>()
					.Which
					.InnerException
					.Should()
					.Be(exception);
			});
		}

		[Fact]
		public void RequestNavigateAsync_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<Internal.Router>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(navigationTarget)).Returns(false);

				sut.Awaiting(x => x.RequestNavigateAsync(region, navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}

		[Fact]
		public void RequestCloseAsync_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<Internal.Router>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(navigationTarget)).Returns(false);

				sut.Awaiting(x => x.RequestCloseAsync(region, navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}
	}
}