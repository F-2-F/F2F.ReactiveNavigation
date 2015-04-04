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
		public void RequestNavigate_WhenCanNavigateToThrowsException_ShouldThrowExceptionAtOriginOfNavigationRequest()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());
				
				var exceptionMessage = Fixture.Create<string>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Throws(new Exception(exceptionMessage));

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();
				// the first navigation request creates a new view model and therefore doesn't call CanNavigateTo
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
			});
		}


		// TODO: Navigation should not return a task. Why should the caller of a navigation request be interest in/informed when a navigation is done or faulted.
		// What would a caller probably do in that case? I am more and more of the opinion, that a caller should not care of the outcome of a navigation request.
		// Let's draw the scenarios:
		// If a caller gets a completed task after navigation was successful, what would be sth. that a caller would want to do in that case??? 
		// If a caller gets a faulted task (incl. the exception) after navigation failed, what would the caller do with that information? It would probably communicate it to the user.
		// But that is sth. that could be done in the navigation target. The navigation target knows much better how to compensate for an exception. 
		// If we propagate and handle that at the call site, there is a strong tendency towards invisible string coupling of unrelated objects due to the fact, that a caller has to handle
		// exceptions of the called object. That is IMHO just bad.
		//
		// If in any case a navigation should return a Task/Observable, then this should only communicate the completion of the request, but not the actual result (success or exception).
		// And this is also only necessary, if a navigation request should be used for indicating busy at the call site. But busy indication is done at the view navigated to,
		// so this is useless as well. So if we really need a completion result, we should pass in a callback in the first time!!
		// I think, we can simplify all of that stuff and the sun will shine bright again on this awesome code base.


		[Fact]
		public void RequestNavigate_WhenNavigatedToThrowsException_ShouldThrowExceptionAtOriginOfNavigationRequest()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				var viewModelFactory = Fixture.Create<ICreateViewModel>();
				var viewModel = A.Fake<ReactiveViewModel>();
				A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).Returns(viewModel.WithUnscopedLifetime());

				var exceptionMessage = Fixture.Create<string>();
				A.CallTo(() => viewModel.CanNavigateTo(A<INavigationParameters>.Ignored)).Throws(new Exception(exceptionMessage));

				Fixture.Inject(viewModelFactory);
				Fixture.Inject<IScheduler>(scheduler);

				var sut = Fixture.Create<Internal.Router>();
				var regionName = Fixture.Create("region");
				sut.AddRegion(regionName);

				var parameters = Fixture.Create<INavigationParameters>();

				// Act
				sut.RequestNavigate<ReactiveViewModel>(regionName, parameters);
			});
		}
	}
}