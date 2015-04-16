using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class NavigableRegion : INavigableRegion
	{
		private readonly Region _region;
		private readonly Router _router;

		public NavigableRegion(Region region, Router router)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");

			_region = region;
			_router = router;
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return _region.Added; }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return _region.Removed; }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return _region.Activated; }
		}

		public Task RequestNavigate<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return _router.RequestNavigate<TViewModel>(_region, parameters);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return _router.RequestNavigate(_region, navigationTarget, parameters);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return _router.RequestClose<TViewModel>(_region, parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return _router.RequestClose(_region, navigationTarget, parameters);
		}
	}
}