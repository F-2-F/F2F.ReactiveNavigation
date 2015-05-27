using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace F2F.ReactiveNavigation.ViewModel
{
	public abstract class ReactiveItemsViewModel<TCollectionItem> : ReactiveValidatedViewModel
		where TCollectionItem : INotifyPropertyChanged
	{
		private ReactiveCommand<Unit> _addItem;
		private ReactiveCommand<Unit> _removeItem;
		private ReactiveCommand<Unit> _clearItems;

		private ReactiveList<TCollectionItem> _items = new ReactiveList<TCollectionItem>();
		private TCollectionItem _selectedItem;

		protected ReactiveItemsViewModel()
		{
		}

		protected internal override void Initialize()
		{
			base.Initialize();

			var canAddItems =
				CanAddItemObservables()
					.CombineLatest()
					.Select(bs => bs.All(b => b))
					.Catch<bool, Exception>(ex =>
					{
						ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(false);
					});

			this.AddItem = ReactiveCommand.CreateAsyncObservable(canAddItems, _ => Observable.Start(() => AddNewItem(), RxApp.MainThreadScheduler));

			var isItemSelected = this.ObservableForProperty(x => x.SelectedItem).Select(x => x.Value != null);

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

			this.RemoveItem = ReactiveCommand.CreateAsyncObservable(canRemoveItems, x => Observable.Start(() => { Remove((TCollectionItem)x); }, RxApp.MainThreadScheduler).Select(_ => Unit.Default));

			var canClearItems =
				CanClearItemsObservables()
					.CombineLatest()
					.Select(bs => bs.All(b => b))
					.Catch<bool, Exception>(ex =>
					{
						ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(false);
					});

			this.ClearItems = ReactiveCommand.CreateAsyncObservable(canClearItems, _ => Observable.Start(() => Items.Clear(), RxApp.MainThreadScheduler));
		}

		public ReactiveCommand<Unit> AddItem
		{
			get { return _addItem; }
			protected set { this.RaiseAndSetIfChanged(ref _addItem, value); }
		}

		public ReactiveCommand<Unit> RemoveItem
		{
			get { return _removeItem; }
			protected set { this.RaiseAndSetIfChanged(ref _removeItem, value); }
		}

		public ReactiveCommand<Unit> ClearItems
		{
			get { return _clearItems; }
			protected set { this.RaiseAndSetIfChanged(ref _clearItems, value); }
		}

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

		private void AddNewItem()
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
		}

		private void Remove(TCollectionItem itemToRemove)
		{
			if (itemToRemove == null)
				throw new ArgumentNullException("itemToRemove", "itemToRemove is null.");

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
		}

		/// <summary>
		/// Creates a new item of type <typeparamref name="TCollectionItem"/>
		/// </summary>
		/// <returns></returns>
		internal protected abstract TCollectionItem CreateItem();

		// TODO: Make this more Rxy
		/// <summary>
		/// Confirms the removal of the given <paramref name="itemToRemove"/>.
		/// </summary>
		/// <param name="itemToRemove">The item to delete</param>
		/// <param name="removeAction">The action to call, when the removal shall be executed</param>
		protected virtual void ConfirmRemoveOf(TCollectionItem itemToRemove, Action<TCollectionItem> removeAction)
		{
			if (itemToRemove == null)
				throw new ArgumentNullException("itemToRemove", "itemToRemove is null.");
			if (removeAction == null)
				throw new ArgumentNullException("removeAction", "removeAction is null.");

			removeAction(itemToRemove);
		}
	}
}
