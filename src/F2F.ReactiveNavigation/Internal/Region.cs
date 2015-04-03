using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Region : IRegion
	{
		private readonly string _name;
		private readonly Internal.IRouter _router;
		private readonly IList<ReactiveViewModel> _containedViewModels = new List<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _added = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _removed = new Subject<ReactiveViewModel>();
		private readonly Subject<ReactiveViewModel> _activated = new Subject<ReactiveViewModel>();

		public Region(string name, Internal.IRouter router)
		{
			dbc.Contract.Requires<ArgumentNullException>(name != null, "name must not be null");
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");

			_name = name;
			_router = router;
		}

		public string Name
		{
			get { return _name; }
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

		public void Add(ReactiveViewModel viewModel)
		{
			_containedViewModels.Add(viewModel);
			_added.OnNext(viewModel);
		}

		public void Remove(ReactiveViewModel viewModel)
		{
			// Should we let the exception be thrown or introduce some handling? Not sure, yet!
			_containedViewModels.Remove(viewModel);
			_removed.OnNext(viewModel);
		}

		public void Activate(ReactiveViewModel viewModel)
		{
			_activated.OnNext(viewModel);
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			return _containedViewModels.Contains(viewModel);
		}

		public IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate)
		{
			return _containedViewModels.Where(predicate);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return _router.RequestNavigate(navigationTarget, this, parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return _router.RequestClose(navigationTarget, this, parameters);
		}
	}
}