using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.Internal;
using F2F.ReactiveNavigation.ViewModel;
using FakeItEasy;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class Region_Test
	{
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		[Fact]
		public void AssertProperNullGuards()
		{
			var assertion = new GuardClauseAssertion(Fixture);
			assertion.Verify(typeof(Region));
		}

		[Fact]
		public void Add_ShouldPushAddedInstanceToAddedObservable()
		{
			var sut = Fixture.Create<Region>();

			ReactiveViewModel addedVm = null;
			var obs = sut.Added.Subscribe(x => addedVm = x);

			var vm = Fixture.Create<ReactiveViewModel>();
			sut.Add(vm);

			addedVm.Should().Be(vm);
		}

		[Fact]
		public void Remove_ShouldPushRemovedInstanceToRemovedObservable()
		{
			var sut = Fixture.Create<Region>();

			ReactiveViewModel removedVm = null;
			sut.Removed.Subscribe(x => removedVm = x);

			var vm = Fixture.Create<ReactiveViewModel>();
			sut.Add(vm);	// must add, before we can remove it
			sut.Remove(vm);

			removedVm.Should().Be(vm);
		}

		[Fact]
		public void Remove_ShouldThrowIfNotContained()
		{
			var sut = Fixture.Create<Region>();
			var vm = Fixture.Create<ReactiveViewModel>();

			sut.Invoking(x => x.Remove(vm)).ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Activate_ShouldPushActivatedInstanceToActivatedObservable()
		{
			var sut = Fixture.Create<Region>();

			ReactiveViewModel activatedVm = null;
			var obs = sut.Activated.Subscribe(x => activatedVm = x);

			var vm = Fixture.Create<ReactiveViewModel>();
			sut.Add(vm);	// must add, before we can activate it
			sut.Activate(vm);

			activatedVm.Should().Be(vm);
		}

		[Fact]
		public void Activate_ShouldThrowIfNotContained()
		{
			var sut = Fixture.Create<Region>();
			var vm = Fixture.Create<ReactiveViewModel>();

			sut.Invoking(x => x.Activate(vm)).ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Contains_ShouldReturnTrueAfterAdd()
		{
			var sut = Fixture.Create<Region>();

			var vm = Fixture.Create<ReactiveViewModel>();
			sut.Add(vm);

			sut.Contains(vm).Should().BeTrue();
		}

		[Fact]
		public void Contains_ShouldReturnFalseAfterRemove()
		{
			var sut = Fixture.Create<Region>();

			var vm = Fixture.Create<ReactiveViewModel>();
			sut.Add(vm);	// must add, before we can remove it
			sut.Remove(vm);

			sut.Contains(vm).Should().BeFalse();
		}

		[Fact]
		public void RequestNavigate_ShouldForwardRequestToRouter()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<Region>();
			sut.Add(navigationTarget);

			sut.RequestNavigate(navigationTarget, parameters);

			A.CallTo(() => router.RequestNavigate(sut, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public void RequestNavigate_ShouldThrowIfNavigationTargetNotContained()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<Region>();

			sut.Invoking(x => x.RequestNavigate(navigationTarget, parameters)).ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void RequestClose_ShouldForwardRequestToRouter()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<Region>();
			sut.Add(navigationTarget);

			sut.RequestClose(navigationTarget, parameters);

			A.CallTo(() => router.RequestClose(sut, navigationTarget, parameters)).MustHaveHappened();
		}

		[Fact]
		public void RequestClose_ShouldThrowIfNavigationTargetNotContained()
		{
			var parameters = Fixture.Create<INavigationParameters>();
			var navigationTarget = Fixture.Create<ReactiveViewModel>();
			var router = Fixture.Create<Internal.IRouter>();
			Fixture.Inject(router);

			var sut = Fixture.Create<Region>();

			sut.Invoking(x => x.RequestClose(navigationTarget, parameters)).ShouldThrow<ArgumentException>();
		}
	}
}