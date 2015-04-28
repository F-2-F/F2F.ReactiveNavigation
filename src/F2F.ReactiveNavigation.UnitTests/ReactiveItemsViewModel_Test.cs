using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using F2F.Testing.Xunit.FakeItEasy;
using Ploeh.AutoFixture.Idioms;
namespace F2F.ReactiveNavigation.UnitTests
{
	public class ReactiveItemsViewModel_Test : AutoMockFeature
	{
		public abstract class TestItemsViewModel : ReactiveItemsViewModel<ReactiveViewModel>
		{
			protected TestItemsViewModel()
			{
			}
		}

		private class DummyItemsViewModel : ReactiveItemsViewModel<ReactiveViewModel>
		{
			private readonly Subject<Unit> _createItemCallTracker;
			private readonly IFixture _fixture;

			public DummyItemsViewModel(IFixture fixture, Subject<Unit> createItemCallTracker)
			{
				_fixture = fixture;
				_createItemCallTracker = createItemCallTracker;
			}

			internal protected override ReactiveViewModel CreateItem()
			{
				_createItemCallTracker.OnNext(Unit.Default);
				return _fixture.Create<ReactiveViewModel>();
			}
		}

		[Fact]
		public void AddItem_ShouldUseCreateItemFactoryMethod()
		{
			new TestScheduler().With(scheduler =>
			{
				var callTracker = new Subject<Unit>();
				Fixture.Inject(callTracker);
				Fixture.Inject(Fixture);
				
				var sut = Fixture.Create<DummyItemsViewModel>();
				sut.InitializeAsync().Schedule(scheduler);
				var trackedCalls = callTracker.CreateCollection();

				sut.AddItem.Execute(null);
				scheduler.Advance();

				trackedCalls.Count.Should().Be(1);
			});
		}
	}
}
