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

namespace F2F.ReactiveNavigation.UnitTests
{
	public class Router_Test : AutoMockFeature
	{
		[Fact]
		public void RequestNavigate_WhenRegionContainsNoViewModel_ShouldCreateNewViewModel()
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
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened();
			});
		}

		[Fact]
		public void RequestNavigate_WhenRegionContainsNoViewModel_ShouldNavigateToNewViewModel()
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
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigate_WhenRegionContainsNoViewModel_ShouldNotCallCanNavigateToOnNewViewModel()
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
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustNotHaveHappened();
			});
		}

		[Fact]
		public void RequestNavigate_WhenRegionContainsViewModel_AndViewModelCannotBeNavigatedTo_ShouldCreateNewViewModel()
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

				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened(Repeated.Exactly.Twice);
			});
		}

		[Fact]
		public void RequestNavigate_WhenRegionContainsViewModel_AndViewModelCanBeNavigatedTo_ShouldCallCanNavigateTo()
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

				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigate_WhenRegionContainsViewModel_AndViewModelCanBeNavigatedTo_ShouldNavigateTo()
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
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(2);
			});
		}

		[Fact]
		public void RequestNavigate_WhenViewModelCanBeNavigatedTo_ShouldCallCanNavigateTo()
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
				sut.RequestNavigate(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigate_WhenViewModelCanBeNavigatedTo_ShouldNavigateTo()
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
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigate(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigate_WhenViewModelCannotBeNavigatedTo_ShouldCallCanNavigateTo()
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
				sut.RequestNavigate(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestNavigate_WhenViewModelCannotBeNavigatedTo_ShouldNotNavigateTo()
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
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigate(region, viewModel, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(0);
			});
		}

		[Fact]
		public void RequestClose_WhenRegionContainsViewModel_ShouldCallCanClose()
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
				sut.RequestClose<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
			});
		}

		[Fact]
		public void RequestClose_WhenRegionContainsViewModel_AndViewModelCanBeClosed_ShouldClose()
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
				var navigations = viewModel.WhenClosed().CreateCollection();

				// Act
				sut.RequestClose<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestClose_WhenRegionContainsViewModel_AndViewModelCannotBeClosed_ShouldNotClose()
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
				var navigations = viewModel.WhenClosed().CreateCollection();

				// Act
				sut.RequestClose<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Assert
				navigations.Count.Should().Be(0);
			});
		}

		[Fact]
		public void RequestClose_WhenRegionContainsNoViewModel_ShouldNotThrow()
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
				sut.Invoking(x => x.RequestClose<ReactiveViewModel>(region, parameters).Schedule(scheduler)).ShouldNotThrow();
			});
		}

		[Fact]
		public void RequestClose_WhenPassingKnownViewModel_ShouldEndLifetimeUsingScope()
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

				sut.RequestNavigate<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				// Act
				sut.RequestClose<ReactiveViewModel>(region, parameters).Schedule(scheduler);

				A.CallTo(() => scope.Dispose()).MustHaveHappened();
			});
		}

		[Fact]
		public async Task RequestNavigate_WhenCanNavigateToThrowsException_ShouldThrowExceptionAtOriginOfNavigationRequest()
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
			await sut.RequestNavigate<ReactiveViewModel>(region, parameters);

			// Act
			sut
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(region, parameters))
				.ShouldThrow<Exception>()
				.Which
				.Should()
				.Be(exception);
		}

		[Fact]
		public void RequestNavigate_WhenNavigatedToThrowsException_ShouldThrowExceptionAtOriginOfNavigationRequest()
		{
			// Arrange
			var viewModelFactory = Fixture.Create<ICreateViewModel>();
			var viewModel = A.Fake<ReactiveViewModel>();
			A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

			var exception = Fixture.Create<Exception>();
			viewModel.WhenNavigatedTo(_ => true, _ => { throw exception; });

			Fixture.Inject(viewModelFactory);

			var sut = Fixture.Create<Internal.Router>();
			var region = Fixture.Create<Internal.Region>();

			var parameters = Fixture.Create<INavigationParameters>();

			// Act
			sut
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(region, parameters))
				.ShouldThrow<Exception>()
				.Which
				.InnerException		// unobserved navigation exceptions are handled by a default handler that wraps the original exception
				.Should()
				.Be(exception);
		}

		[Fact]
		public void InitializeAsync_WhenInitializeThrowsException_ShouldThrowExceptionAtOrigin()
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
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(region, parameters))
				.ShouldThrow<Exception>()
				.Which
				.Should()
				.Be(exception);
		}

		[Fact]
		public void RequestNavigate_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<Internal.Router>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(navigationTarget)).Returns(false);

				sut.Awaiting(x => x.RequestNavigate(region, navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}

		[Fact]
		public void RequestClose_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<Internal.Router>();

				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();

				var region = Fixture.Create<Internal.IRegion>();
				A.CallTo(() => region.Contains(navigationTarget)).Returns(false);

				sut.Awaiting(x => x.RequestClose(region, navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}
	}
}