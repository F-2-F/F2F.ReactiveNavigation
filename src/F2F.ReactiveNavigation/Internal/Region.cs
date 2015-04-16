using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Region : IRegion, IObserveRegion, IDisposable
	{
		private readonly IRouter _router;
		private readonly ICreateViewModel _viewModelFactory;

		private readonly Subject<ReactiveViewModel> _added = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _removed = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _activated = new Subject<ReactiveViewModel>();

		private readonly ConcurrentDictionary<ReactiveViewModel, IDisposable> _ownedViewModels
			= new ConcurrentDictionary<ReactiveViewModel, IDisposable>();

		private bool _disposed = false;

		public Region(IRouter router, ICreateViewModel viewModelFactory)
		{
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");
			dbc.Contract.Requires<ArgumentNullException>(viewModelFactory != null, "viewModelFactory must not be null");

			_router = router;
			_viewModelFactory = viewModelFactory;
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return _added; }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return _removed; }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return _activated; }
		}

		public TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel
		{
			var scopedTarget = _viewModelFactory.CreateViewModel<TViewModel>();
			var navigationTarget = scopedTarget.Object;

			_ownedViewModels.AddOrUpdate(navigationTarget, scopedTarget, (t, h) => scopedTarget);

			_added.OnNext(navigationTarget);

			return navigationTarget;
		}

		public void Remove(ReactiveViewModel viewModel)
		{
			IDisposable scopedTarget;
			if (_ownedViewModels.TryRemove(viewModel, out scopedTarget))
			{
				scopedTarget.Dispose();
			}

			_removed.OnNext(viewModel);
		}

		public void Activate(ReactiveViewModel viewModel)
		{
			_activated.OnNext(viewModel);
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			return _ownedViewModels.Keys.Contains(viewModel);
		}

		public IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate)
		{
			return _ownedViewModels.Keys.Where(predicate);
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