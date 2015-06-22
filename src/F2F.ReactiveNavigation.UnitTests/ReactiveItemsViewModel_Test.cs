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
		public class ThrowingItemsViewModel : ReactiveItemsViewModel<ReactiveViewModel>
		{
			private readonly Exception _throwThis;

			public ThrowingItemsViewModel(Exception throwThis)
			{
				_throwThis = throwThis;
			}

			protected internal override Task<ReactiveViewModel> CreateItem()
			{
				throw _throwThis;
			}
		}

		private class ConfigurableItemsViewModel : ReactiveItemsViewModel<ReactiveViewModel>
		{
			private readonly Subject<Unit> _createItemCallTracker;
			private readonly IFixture _fixture;
			private readonly Subject<bool> _canAddItem;

			public ConfigurableItemsViewModel(IFixture fixture, Subject<Unit> createItemCallTracker, Subject<bool> canAddItem)
			{
				_fixture = fixture;
				_createItemCallTracker = createItemCallTracker;
				_canAddItem = canAddItem;
			}

			internal protected override Task<ReactiveViewModel> CreateItem()
			{
				_createItemCallTracker.OnNext(Unit.Default);
				return Task.FromResult(_fixture.Create<ReactiveViewModel>());
			}

			protected internal override IEnumerable<IObservable<bool>> CanAddItemObservables()
			{
				yield return _canAddItem;
				foreach(var o in base.CanAddItemObservables())
					yield return o;
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

				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);
				var trackedCalls = callTracker.CreateCollection();

				sut.AddItem.Execute(null);
				scheduler.Advance();

				trackedCalls.Count.Should().Be(1);
			});
		}


		[Fact]
		public void AddItem_WhenCalled_ShouldHaveItemsCountEqualToOne()
		{
			new TestScheduler().With(scheduler =>
			{
				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);

				sut.AddItem.Execute(null);
				scheduler.Advance();

				sut.Items.Count.Should().Be(1);
			});
		}

		[Fact]
		public void AddItem_WhenCanAddItemObservableYieldsFalse_ShouldReturnFalseForCanExecute()
		{
			new TestScheduler().With(scheduler =>
			{
				var canAddItem = new Subject<bool>();
				Fixture.Inject(canAddItem);
				Fixture.Inject(Fixture);

				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);
				canAddItem.OnNext(false);

				sut.AddItem.CanExecute(null).Should().BeFalse();
			});
		}

		[Fact]
		public void AddItem_WhenCanAddItemObservableYieldsFalse_ShouldAddItemRegardlessOfCanAddItemObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var canAddItem = new Subject<bool>();
				Fixture.Inject(canAddItem);
				Fixture.Inject(Fixture);

				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);
				canAddItem.OnNext(false);	// yield false for CanAdd

				sut.AddItem.Execute(null);
				scheduler.Advance();

				sut.Items.Count.Should().Be(1);
			});
		}

		[Fact]
		public void CanAddItem_WhenUnobservedExceptionIsThrown_ShouldThrowAtCallSite()
		{
			new TestScheduler().With(scheduler =>
			{
				var canAddItem = new Subject<bool>();
				Fixture.Inject(canAddItem);
				Fixture.Inject(Fixture);

				var ex = Fixture.Create<Exception>();
				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);
				canAddItem.OnError(ex);

				sut.AddItem.Execute(null);

				scheduler.Invoking(sched => sched.Advance()).ShouldThrow<Exception>().Which.InnerException.Should().Be(ex);
			});
		}

		[Fact]
		public void CanAddItem_WhenObservedExceptionIsThrown_ShouldPushExceptionToThrownExceptionsObservable()
		{
			new TestScheduler().With(scheduler =>
			{
				var canAddItem = new Subject<bool>();
				Fixture.Inject(canAddItem);
				Fixture.Inject(Fixture);

				var ex = Fixture.Create<Exception>();
				var sut = Fixture.Build<ConfigurableItemsViewModel>().OmitAutoProperties().Create();
				sut.InitializeAsync().Schedule(scheduler);

				var observedExceptions = sut.ThrownExceptions.CreateCollection();

				canAddItem.OnError(ex);
				sut.AddItem.Execute(null);
				scheduler.Advance();

				observedExceptions.Single().Should().Be(ex);
			});
		}

		// The following 2 tests effectively test ReactiveCommand's exception policy. We don't need to test that!
		// I keep the tests here for a marker to think about piping the command's exceptions to the ThrownExceptionSource of the view model.

		//[Fact]
		//public void AddItem_WhenUnobservedExceptionIsThrown_ShouldThrowAtCallSite()
		//{
		//	new TestScheduler().With(scheduler =>
		//	{
		//		var ex = Fixture.Create<Exception>();
		//		Fixture.Inject(ex);

		//		var sut = Fixture.Create<ThrowingItemsViewModel>();
		//		sut.InitializeAsync().Schedule(scheduler);

		//		sut.AddItem.Execute(null);

		//		scheduler.Invoking(sched => sched.Advance()).ShouldThrow<Exception>().Which.InnerException.Should().Be(ex);
		//	});
		//}

		//[Fact]
		//public void AddItem_WhenObservedExceptionIsThrown_ShouldPushExceptionTo()
		//{
		//	new TestScheduler().With(scheduler =>
		//	{
		//		var ex = Fixture.Create<Exception>();
		//		Fixture.Inject(ex);

		//		var sut = Fixture.Create<ThrowingItemsViewModel>();
		//		sut.InitializeAsync().Schedule(scheduler);

		//		var observedExceptions = sut.AddItem.ThrownExceptions.CreateCollection();
				
		//		sut.AddItem.Execute(null);
		//		scheduler.Advance();

		//		observedExceptions.Single().Should().Be(ex);
		//	});
		//}

	}
}
