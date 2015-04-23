using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace F2F.ReactiveNavigation.ViewModel
{
	public abstract class ReactiveItemsViewModel<TCollectionItem> : ReactiveValidatedViewModel
	{
		private ReactiveList<TCollectionItem> _items;
		private TCollectionItem _selectedItem;
		
		protected ReactiveItemsViewModel()
		{
		}

		protected internal override void Initialize()
		{
			base.Initialize();

			var isItemSelected = this.ObservableForProperty(x => x.SelectedItem).Select(x => x.Value != null);

			var canAddItems =
				CanAddItemObservables()
					.Concat(new[] { isItemSelected })
					.CombineLatest()
					.Select(bs => bs.All(b => b))
					.Catch<bool, Exception>(ex =>
					{
						ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(false);
					});

			this.AddItem =
				ReactiveCommand.CreateAsyncObservable(
					canAddItems,
					_ => Observable.Start(() =>
					{
						var currentItem = SelectedItem;
						var newItem = CreateItem();

						if (currentItem != null)
						{
							// insert new item directly after currentItem
							var newItemIndex = 1 + _items.IndexOf(currentItem);
							Items.Insert(newItemIndex, newItem);
						}
						else
						{
							Items.Add(newItem);
						}

						SelectedItem = newItem;

					}, RxApp.MainThreadScheduler));

			var canRemoveItems =
				CanRemoveItemObservables()
					.Concat(new[] { isItemSelected })
					.CombineLatest()
					.Select(bs => bs.All(b => b))
					.Catch<bool, Exception>(ex =>
					{
						ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(false);
					});

			this.RemoveItem =
				ReactiveCommand.CreateAsyncObservable(
					canRemoveItems,
					x => Observable.Start(() =>
					{
						var itemToRemove = (TCollectionItem)x;

						Action<TCollectionItem> removeItem = item =>
						{
							var removedItemIndex = this.Items.IndexOf(item);
							Items.Remove(item);

							if (Items.Count > 0)
							{
								var newSelectedIndex = Math.Max(0, Math.Min(Items.Count - 1, removedItemIndex));
								SelectedItem = _items[newSelectedIndex];
							}
						};

						ConfirmRemoveOf(itemToRemove, removeItem);
					}, RxApp.MainThreadScheduler)
						.Select(_ => Unit.Default));

			var containsAtLeastOneItem = this.Items.CountChanged.Select(count => count > 0);

			var canClearItems =
				CanClearItemsObservables()
					.Concat(new[] { containsAtLeastOneItem })
					.CombineLatest()
					.Select(bs => bs.All(b => b))
					.Catch<bool, Exception>(ex =>
					{
						ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(false);
					});

			this.ClearItems =
				ReactiveCommand.CreateAsyncObservable(
					canClearItems,
					_ => Observable.Start(() => Items.Clear(), RxApp.MainThreadScheduler));
		}


		public ReactiveCommand<Unit> AddItem { get; protected set; }

		public ReactiveCommand<Unit> RemoveItem { get; protected set; }

		public ReactiveCommand<Unit> ClearItems { get; protected set; }

		public TCollectionItem SelectedItem
		{
			get { return _selectedItem; }
			set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
		}

		public ReactiveList<TCollectionItem> Items
		{
			get { return _items; }
			protected set { this.RaiseAndSetIfChanged(ref _items, value); }
		}

		internal protected virtual IEnumerable<IObservable<bool>> CanAddItemObservables()
		{
			yield return Observable.Return(true);
		}

		internal protected virtual IEnumerable<IObservable<bool>> CanRemoveItemObservables()
		{
			yield return Observable.Return(true);
		}

		internal protected virtual IEnumerable<IObservable<bool>> CanClearItemsObservables()
		{
			yield return Observable.Return(true);
		}

		/// <summary>
		/// Creates a new item of type <typeparamref name="TCollectionItem"/>
		/// </summary>
		/// <returns></returns>
		protected abstract TCollectionItem CreateItem();

		/// <summary>
		/// Confirms the removal of the given <paramref name="itemToRemove"/>.
		/// </summary>
		/// <param name="itemToRemove">The item to delete</param>
		/// <param name="removeAction">The action to call, when the removal shall be executed</param>
		protected virtual void ConfirmRemoveOf(TCollectionItem itemToRemove, Action<TCollectionItem> removeAction)
		{
			removeAction(itemToRemove);
		}
	}
}
