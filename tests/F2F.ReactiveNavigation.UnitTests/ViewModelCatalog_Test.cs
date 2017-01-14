using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Xunit;
using F2F.ReactiveNavigation.ViewModel;
using F2F.Testing.Xunit.FakeItEasy;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class ViewModelCatalog_Test : AutoMockFeature
    {
        private class DummyView { }

        [Fact]
        private void CreateViewFor_WhenRegistered_ShouldNotReturnNull()
        {
            var sut = Fixture.Create<ViewFactory>();

            sut.Register<ReactiveViewModel>(_ => Fixture.Create<object>());

            sut.CreateViewFor(Fixture.Create<ReactiveViewModel>()).Should().NotBeNull();
        }

        [Fact]
        private void CreateViewFor_WhenRegistered_ShouldReturnCorrectView()
        {
            var sut = Fixture.Create<ViewFactory>();

            sut.Register<ReactiveViewModel>(_ => Fixture.Create<DummyView>());

            sut.CreateViewFor(Fixture.Create<ReactiveViewModel>()).Should().BeOfType<DummyView>();
        }

        [Fact]
        public void CreateViewFor_NonRegisteredViewModel_ShouldThrow()
        {
            var sut = Fixture.Create<ViewFactory>();

            sut.Invoking(x => x.CreateViewFor(Fixture.Create<ReactiveViewModel>())).ShouldThrow<Exception>();
        }
    }
}