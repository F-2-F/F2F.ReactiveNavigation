using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.Internal;
using F2F.ReactiveNavigation.ViewModel;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using F2F.Testing.Xunit.FakeItEasy;
using Xunit.Extensions;
using FakeItEasy;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class Region_Test : AutoMockFeature
    {
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

            var vm = sut.Add<ReactiveViewModel>();

            addedVm.Should().Be(vm);
        }

        [Fact]
        public void Add_ShouldUseViewModelFactoryToCreateViewModels()
        {
            var viewModelFactory = Fixture.Create<ICreateViewModel>();

            Fixture.Inject(viewModelFactory);

            var sut = Fixture.Create<Region>();

            sut.Add<ReactiveViewModel>();

            A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Remove_ShouldPushRemovedInstanceToRemovedObservable()
        {
            var sut = Fixture.Create<Region>();

            ReactiveViewModel removedVm = null;
            sut.Removed.Subscribe(x => removedVm = x);

            var vm = sut.Add<ReactiveViewModel>();    // must add, before we can remove it
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

            var vm = sut.Add<ReactiveViewModel>();    // must add, before we can activate it
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
        public void Deactivate_ShouldPushDeactivatedInstanceToDeactivatedObservable()
        {
            var sut = Fixture.Create<Region>();

            ReactiveViewModel deactivatedVm = null;
            var obs = sut.Deactivated.Subscribe(x => deactivatedVm = x);

            var vm = sut.Add<ReactiveViewModel>();    // must add, before we can deactivate it
            sut.Deactivate(vm);

            deactivatedVm.Should().Be(vm);
        }

        [Fact]
        public void Deactivate_ShouldThrowIfNotContained()
        {
            var sut = Fixture.Create<Region>();
            var vm = Fixture.Create<ReactiveViewModel>();

            sut.Invoking(x => x.Deactivate(vm)).ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Contains_ShouldReturnTrueAfterAdd()
        {
            var sut = Fixture.Create<Region>();

            var vm = sut.Add<ReactiveViewModel>();

            sut.Contains(vm).Should().BeTrue();
        }

        [Fact]
        public void Contains_ShouldReturnFalseAfterRemove()
        {
            var sut = Fixture.Create<Region>();

            var vm = sut.Add<ReactiveViewModel>();        // must add, before we can remove it
            sut.Remove(vm);

            sut.Contains(vm).Should().BeFalse();
        }

        [Theory, AutoMockData]
        public void ViewModels_ShouldReturnAllAddedViewModels(int howMany)
        {
            var sut = Fixture.Create<Region>();
            var addedViewModels = new List<ReactiveViewModel>();

            for (int i = 0; i < howMany; i++)
            {
                addedViewModels.Add(sut.Add<ReactiveViewModel>());
            }

            sut.ViewModels.ShouldAllBeEquivalentTo(addedViewModels);
        }


        [Theory, AutoMockData]
        public void Dispose_ShouldDisposeAllViewModelScopes(int howMany)
        {
            var scope = Fixture.Create<IDisposable>();
            var viewModelFactory = Fixture.Create<ICreateViewModel>();

            A.CallTo(() => viewModelFactory.CreateViewModel<ReactiveViewModel>()).ReturnsLazily(() => Scope.From(Fixture.Create<ReactiveViewModel>(), scope));

            Fixture.Inject(viewModelFactory);

            var sut = Fixture.Create<Region>();

            for (int i = 0; i < howMany; i++)
            {
                sut.Add<ReactiveViewModel>();
            }

            sut.Dispose();
            A.CallTo(() => scope.Dispose()).MustHaveHappened(Repeated.Exactly.Times(howMany));
        }
    }
}