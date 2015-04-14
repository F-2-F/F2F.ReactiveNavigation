using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.Internal;
using F2F.ReactiveNavigation.ViewModel;
using FakeItEasy;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class Navigator_Test
	{
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		[Fact]
		public void AssertProperNullGuards()
		{
			var assertion = new GuardClauseAssertion(Fixture);
			assertion.Verify(typeof(Navigator));
		}

		[Fact]
		public void RequestNavigate_ShouldForwardRequestToRouter()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var region = Fixture.Create<Internal.IRegion>();
			Fixture.Inject(region);
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<Navigator>();

			sut.RequestNavigate(navigationTarget, parameters);

			A.CallTo(() => router.RequestNavigate(region, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public void RequestNavigate_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();
				var router = Fixture.Create<Internal.IRouter>();
				Fixture.Inject(router);

				var sut = Fixture.Create<Navigator>();

				sut.Invoking(x => x.RequestNavigate(navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}

		[Fact]
		public void RequestClose_ShouldForwardRequestToRouter()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var region = Fixture.Create<Internal.IRegion>();
			Fixture.Inject(region);
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);
			var navigationTarget = region.Add<ReactiveViewModel>();

			var sut = Fixture.Create<Navigator>();

			sut.RequestClose(navigationTarget, parameters);

			A.CallTo(() => router.RequestClose(region, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public void RequestClose_ShouldThrowIfNavigationTargetNotContained()
		{
			new TestScheduler().With(scheduler =>
			{
				var parameters = Fixture.Create<INavigationParameters>();
				var navigationTarget = Fixture.Create<ReactiveViewModel>();
				var router = Fixture.Create<Internal.IRouter>();
				Fixture.Inject(router);

				var sut = Fixture.Create<Navigator>();

				sut.Invoking(x => x.RequestClose(navigationTarget, parameters).Schedule(scheduler)).ShouldThrow<ArgumentException>();
			});
		}
	}
}
