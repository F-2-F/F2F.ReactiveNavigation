using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal abstract class Region : IRegion, IObserveRegion, IDisposable
	{
		private readonly ICreateViewModel _viewModelFactory;

		private readonly Subject<ReactiveViewModel> _added = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _removed = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _activated = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _deactivated = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _initialized = new Subject<ReactiveViewModel>();

		private readonly ConcurrentDictionary<ReactiveViewModel, IDisposable> _ownedViewModels
			= new ConcurrentDictionary<ReactiveViewModel, IDisposable>();

		private bool _disposed = false;

		protected Region(ICreateViewModel viewModelFactory)
		{
			if (viewModelFactory == null)
				throw new ArgumentNullException("viewModelFactory", "viewModelFactory is null.");

			_viewModelFactory = viewModelFactory;
		}

		internal ICreateViewModel ViewModelFactory
		{
			get { return _viewModelFactory; }
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return _added; }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return _activated; }
		}

		public IObservable<ReactiveViewModel> Initialized
		{
			get { return _initialized; }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return _removed; }
		}

		public IObservable<ReactiveViewModel> Deactivated
		{
			get { return _deactivated; }
		}

		public IEnumerable<ReactiveViewModel> ViewModels
		{
			get { return _ownedViewModels.Keys; }
		}

		public TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel
		{
			var scopedTarget = ViewModelFactory.CreateViewModel<TViewModel>();
			var navigationTarget = scopedTarget.Object;

			Adding(navigationTarget);

			_ownedViewModels.AddOrUpdate(navigationTarget, scopedTarget, (t, h) => scopedTarget);

			_added.OnNext(navigationTarget);

			return navigationTarget;
		}

		internal protected virtual void Adding<TViewModel>(TViewModel itemToBeAdded)
		{
		}

		public void Remove(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");
			if (!Contains(viewModel))
				throw new ArgumentException("viewModel does not belong to region");

			IDisposable scopedTarget;
			if (_ownedViewModels.TryRemove(viewModel, out scopedTarget))
			{
				scopedTarget.Dispose();
				_removed.OnNext(viewModel);
			}
		}

		public void Activate(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");
			if (!Contains(viewModel))
				throw new ArgumentException("viewModel does not belong to region");

			_activated.OnNext(viewModel);
		}

		public void Initialize(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");
			if (!Contains(viewModel))
				throw new ArgumentException("viewModel does not belong to region");

			_initialized.OnNext(viewModel);
		}

		public void Deactivate(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");
			if (!Contains(viewModel))
				throw new ArgumentException("viewModel does not belong to region");

			_deactivated.OnNext(viewModel);
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");

			return _ownedViewModels.Keys.Contains(viewModel);
		}

		public IEnumerable<TViewModel> Find<TViewModel>(Func<TViewModel, bool> predicate)
			where TViewModel : ReactiveViewModel
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate", "predicate is null.");

			return _ownedViewModels.Keys.OfType<TViewModel>().Where(predicate);
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				foreach (var vm in _ownedViewModels)
				{
					vm.Value.Dispose();
				}

				_disposed = true;
			}
		}
	}
}