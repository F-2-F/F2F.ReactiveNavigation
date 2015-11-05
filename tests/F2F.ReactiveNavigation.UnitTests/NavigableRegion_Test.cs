using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.Internal;
using F2F.ReactiveNavigation.ViewModel;
using F2F.Testing.Xunit.FakeItEasy;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class NavigableRegion_Test : AutoMockFeature
	{
		[Fact(Skip = "GuardClauseAssertion does not work with async methods (https://github.com/AutoFixture/AutoFixture/issues/268)")]
		public void AssertProperNullGuards()
		{
			var assertion = new GuardClauseAssertion(Fixture);
			assertion.Verify(typeof(NavigableRegion));
		}

		[Fact]
		public async Task RequestNavigate_WithViewModelInstance_ShouldForwardRequestToRouter()
		{
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<NavigableRegion>();

			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var parameters = Fixture.Create<INavigationParameters>();

			await sut.RequestNavigate(navigationTarget, parameters);

			A.CallTo(() => router.RequestNavigateAsync(sut.Region, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public async Task RequestNavigate_WithViewModelType_ShouldForwardRequestToRouter()
		{
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<NavigableRegion>();

			var parameters = Fixture.Create<INavigationParameters>();

			await sut.RequestNavigate<ReactiveViewModel>(parameters);

			A.CallTo(() => router.RequestNavigateAsync<ReactiveViewModel>(sut.Region, parameters)).MustHaveHappened();
		}

		[Fact]
		public async Task RequestClose_WithViewModelInstance_ShouldForwardRequestToRouter()
		{
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<NavigableRegion>();

			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var parameters = Fixture.Create<INavigationParameters>();

			await sut.RequestClose(navigationTarget, parameters);

			A.CallTo(() => router.RequestCloseAsync(sut.Region, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public async Task RequestClose_WithViewModelType_ShouldForwardRequestToRouter()
		{
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<NavigableRegion>();

			var parameters = Fixture.Create<INavigationParameters>();

			await sut.RequestClose<ReactiveViewModel>(parameters);

			A.CallTo(() => router.RequestCloseAsync<ReactiveViewModel>(sut.Region, parameters)).MustHaveHappened();
		}

		[Theory]
		[AutoMockData]
		public async Task CloseAll_ShouldRequestCloseForAllContainedViewModels(int viewModelCount)
		{
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var region = Fixture.Create<MultiItemsRegion>();
			IList<ReactiveViewModel> viewModels = new List<ReactiveViewModel>();

			for (int i = 0; i < viewModelCount; i++)
			{
				viewModels.Add(region.Add<ReactiveViewModel>());
			}

			Fixture.Inject<Region>(region);

			var sut = Fixture.Create<NavigableRegion>();

			await sut.CloseAll();

			foreach (var vm in viewModels)
			{
				A.CallTo(() => router.RequestCloseAsync(sut.Region, vm, NavigationParameters.CloseRegion)).MustHaveHappened();
			}
		}
	}
}