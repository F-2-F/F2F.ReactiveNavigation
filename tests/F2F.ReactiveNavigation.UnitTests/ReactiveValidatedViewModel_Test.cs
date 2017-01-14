using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using FluentAssertions;
using FluentValidation;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using ReactiveUI;
using Xunit;
using F2F.Testing.Xunit.FakeItEasy;
using Ploeh.AutoFixture.Idioms;
using Ploeh.Albedo;

namespace F2F.ReactiveNavigation.UnitTests
{
    public class ReactiveValidatedViewModel_Test : AutoMockFeature
    {
        private class TestViewModel : ReactiveValidatedViewModel
        {
            private class Validator : AbstractValidator<TestViewModel>
            {
                public Validator()
                {
                    RuleFor(x => x.StringProperty).NotEmpty();
                    RuleFor(x => x.IntProperty).GreaterThanOrEqualTo(0);
                }
            }

            private string _stringProperty;

            public string StringProperty
            {
                get { return _stringProperty; }
                set { this.RaiseAndSetIfChanged(ref _stringProperty, value); }
            }

            private int _intProperty;

            public int IntProperty
            {
                get { return _intProperty; }
                set { this.RaiseAndSetIfChanged(ref _intProperty, value); }
            }

            protected override IValidator ProvideValidator()
            {
                return new Validator();
            }
        }

        [Fact]
        public async Task HasErrors_ForValidViewModel_ShouldReturnFalse()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.HasErrors.Should().BeFalse();
            sut.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task HasErrors_InvalidPropertyBeforeInitialization_ShouldReturnTrue()
        {
            Fixture.Inject(-1 * Fixture.Create<int>());
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.HasErrors.Should().BeTrue();
            sut.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task HasErrors_InvalidPropertyAfterInitialization_ShouldReturnTrue()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.IntProperty = -1 * Fixture.Create<int>();    // inject negative value
            sut.HasErrors.Should().BeTrue();
            sut.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task HasErrors_ShouldReturnTrueAsLongAsPropertiesAreInError()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.IntProperty = -1 * Fixture.Create<int>();
            sut.StringProperty = null;

            sut.HasErrors.Should().BeTrue();
            sut.IsValid.Should().BeFalse();

            sut.StringProperty = Fixture.Create<string>();    // fix one error
            sut.HasErrors.Should().BeTrue();
            sut.IsValid.Should().BeFalse();

            sut.IntProperty = Fixture.Create<int>();    // fix last error
            sut.HasErrors.Should().BeFalse();
            sut.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task GetErrors_ForValidViewModel_ShouldReturnEmptyEnumerable()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.GetErrors(sut.GetPropertyName(x => x.IntProperty)).Should().BeEmpty();
            sut.GetErrors(sut.GetPropertyName(x => x.StringProperty)).Should().BeEmpty();
        }

        [Fact]
        public async Task GetErrors_InvalidProperties_ShouldReturnErrorsForEveryPropertyInError()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            sut.IntProperty = -1 * Fixture.Create<int>();
            sut.StringProperty = null;

            sut.GetErrors(sut.GetPropertyName(x => x.IntProperty)).Should().NotBeEmpty();
            sut.GetErrors(sut.GetPropertyName(x => x.StringProperty)).Should().NotBeEmpty();
        }

        [Fact]
        public async Task ErrorsChanged_ShouldBeRaisedWhenPropertyErrorChanged()
        {
            var sut = Fixture.Create<TestViewModel>();
            await sut.InitializeAsync();

            int count = 0;
            using (Observable.FromEventPattern(sut, "ErrorsChanged").Do(_ => count++).Subscribe())
            {
                var rounds = 2;
                string invalidValue = null;
                string validValue = Fixture.Create<string>();
                for (int i = 0; i < rounds; i++)
                {
                    sut.StringProperty = invalidValue;
                    sut.StringProperty = validValue;
                }

                count.Should().Be(2 * rounds);
            }
        }

        // TODO Add tests for Initialization throwing exceptions
    }
}