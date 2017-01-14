using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.Internal;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using FluentAssertions;
using F2F.ReactiveNavigation.ViewModel;
using Xunit.Extensions;
using F2F.Testing.Xunit.FakeItEasy;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class SingleItemRegion_Test : AutoMockFeature
    {
        [Fact]
        public void AssertProperNullGuards()
        {
            var assertion = new GuardClauseAssertion(Fixture);
            assertion.Verify(typeof(MultiItemsRegion));
        }

        [Fact]
        public void Add_WhenRegionAlreadyContainsAnItem_ShouldRemoveContainedItem()
        {
            var sut = Fixture.Create<SingleItemRegion>();

            ReactiveViewModel removedVm = null;
            using (var obs = sut.Removed.Subscribe(x => removedVm = x))
            {
                var vm = sut.Add<ReactiveViewModel>();
                sut.Add<ReactiveViewModel>();

                removedVm.Should().Be(vm);
            }
        }

        [Theory, AutoMockData]
        public void Add_WhenCalledMultipleItems_ShouldAlwaysContainLastAddedItem(int howOften)
        {
            var sut = Fixture.Create<SingleItemRegion>();
            for (int i = 0; i < howOften; i++)
            {
                var vm = sut.Add<ReactiveViewModel>();
                sut.ViewModels.Single().Should().Be(vm);
            }
        }
    }
}
