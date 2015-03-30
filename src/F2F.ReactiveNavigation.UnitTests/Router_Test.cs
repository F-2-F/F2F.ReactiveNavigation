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

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Assert
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened();
				A.CallTo(() => viewModel.NavigatedTo(A<INavigationParameters>.Ignored)).MustHaveHappened();
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

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Assert
				A.CallTo(() => viewModel.NavigatedTo(A<INavigationParameters>.Ignored)).MustHaveHappened();
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
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

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

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => viewModel.NavigatedTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);
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

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Act
				sut.RequestClose<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Assert
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => viewModel.NavigatedTo(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
				A.CallTo(() => viewModel.CanClose(A<INavigationParameters>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
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

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

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

				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				// Act
				sut.RequestClose<ReactiveViewModel>(regionName, parameters);
				scheduler.AdvanceByMs(1);

				A.CallTo(() => scope.Dispose()).MustHaveHappened();
			});
		}

	}
}