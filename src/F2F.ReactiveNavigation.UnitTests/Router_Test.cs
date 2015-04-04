using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Xunit;
using F2F.ReactiveNavigation.ViewModel;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class Router_Test
	{
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		[Fact]
		public void RequestNavigate_WhenPassingUnknownViewModel_ShouldCreateNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened();
				navigations.Count.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigate_WhenPassingUnknownViewModel_ShouldCallNavigatedToOnNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();
				int navigations = 0;
				viewModel.WhenNavigatedTo().Subscribe(_ => navigations++);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);
				
				// Assert
				navigations.Should().Be(1);
			});
		}

		[Fact]
		public void RequestNavigate_WhenPassingUnknownViewModel_ShouldNotCallCanNavigateToOnNewViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustNotHaveHappened();
			});
		}

		[Fact]
		public void RequestNavigate_WhenPassingKnownViewModel_ShouldNavigateToViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				navigations.Count.Should().Be(2);
			});
		}

		[Fact]
		public void RequestClose_WhenPassingKnownViewModel_ShouldCloseViewModel()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();
				var navigations = viewModel.WhenNavigatedTo().CreateCollection();

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Act
				sut.RequestClose<ReactiveViewModel>(regionName, parameters);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				navigations.Count.Should().Be(1);
			});
		}


		private class DummyViewModel : ReactiveViewModel { }

		//TODO: think about rather throwing an exception for an unknown view model !!
		[Fact]
		public void RequestClose_WhenPassingUnknownViewModel_ShouldNotThrow()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Returns(true);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).Returns(true);

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Act
				sut.Invoking(x => x.RequestClose<DummyViewModel>(regionName, parameters)).ShouldNotThrow();
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
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters).Schedule(scheduler);

				// Act
				sut.RequestClose<ReactiveViewModel>(regionName, parameters);
				scheduler.Advance();

				A.CallTo(() => scope.Dispose()).MustHaveHappened();
			});
		}

		// TODO Add tests for initialization / navigation / busy throws exception

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
			var regionName = Fixture.Create("region");
			sut.AddRegion(regionName);

			var parameters = Fixture.Create<INavigationParameters>();
			// the first navigation request creates a new view model and therefore doesn't call CanNavigateTo
			await sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);

			// Act
			sut
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(regionName, parameters))
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
			var regionName = Fixture.Create("region");
			sut.AddRegion(regionName);

			var parameters = Fixture.Create<INavigationParameters>();

			// Act
			sut
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(regionName, parameters))
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
			var regionName = Fixture.Create("region");
			sut.AddRegion(regionName);

			var parameters = Fixture.Create<INavigationParameters>();
			
			// Act
			sut
				.Awaiting(x => x.RequestNavigate<ReactiveViewModel>(regionName, parameters))
				.ShouldThrow<Exception>()
				.Which
				.Should()
				.Be(exception);
		}
	}
}