using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using F2F.ReactiveNavigation.ViewModel;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;

namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveViewModel_Test
	{
		private class TestViewModel : ReactiveViewModel
		{
			public TestViewModel()
			{
			}

			protected internal override void Initialize()
			{
				base.Initialize();

				// throw an exception whenever title property changes
				this.ObservableForProperty(x => x.Title)
					.Do(_ => { throw new Exception(); })
					.Subscribe();
			}
		}

		private readonly IFixture Fixture;

		public ReactiveViewModel_Test()
		{
			Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
		}


		[Fact]
		public void ThrownExceptions_ShouldAlsoPushThrownExceptionsFromBaseClass()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Create<TestViewModel>();
				var thrownExceptions = sut.ThrownExceptions.CreateCollection();

				sut.InitializeAsync();
				scheduler.Advance();	// schedule initialization

				sut.Title = Fixture.Create<string>();
				scheduler.Advance();

				thrownExceptions.Should().HaveCount(1);
			});
		}
	}
}
