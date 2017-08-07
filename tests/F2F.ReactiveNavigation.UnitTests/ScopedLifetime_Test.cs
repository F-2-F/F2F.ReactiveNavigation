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
using System.Reactive.Disposables;
using Ploeh.AutoFixture.Idioms;
using F2F.Testing.Xunit.FakeItEasy;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class ScopedLifetime_Test : AutoMockFeature
    {
        [Fact]
        public void Dispose_ShouldCallDisposeOnScope()
        {
            var disposable = Fixture.Create<IDisposable>();
            Fixture.Inject(disposable);

            var sut = Fixture.Create<ScopedLifetime<object>>();

            sut.Dispose();
            
            A.CallTo(() => disposable.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void AssertProperNullGuards()
        {
            var assertion = new GuardClauseAssertion(Fixture);
            assertion.Verify(typeof(ScopedLifetime<object>));
        }

        [Fact]
        public void AssertProperInitialization()
        {
            var assertion = new ConstructorInitializedMemberAssertion(Fixture);
            assertion.Verify(typeof(ScopedLifetime<object>));
        }
    }
}
