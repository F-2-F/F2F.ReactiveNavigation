using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class NavigableRegion : INavigableRegion, ICloseRegion
	{
		private readonly Region _region;
		private readonly IRouter _router;

		public NavigableRegion(Region region, IRouter router)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");

			_region = region;
			_router = router;
		}

		public Region Region
		{
			get { return _region; }
		}

		public IRouter Router
		{
			get { return _router; }
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return Region.Added; }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return Region.Removed; }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return Region.Activated; }
		}

		public Task RequestNavigate<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return Router.RequestNavigate<TViewModel>(Region, parameters);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return Router.RequestNavigate(Region, navigationTarget, parameters);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return Router.RequestClose<TViewModel>(Region, parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return Router.RequestClose(Region, navigationTarget, parameters);
		}

		public async Task CloseAll()
		{
			foreach (var vm in Region.Find(_ => true))
			{
				await Router.RequestClose(Region, vm, NavigationParameters.CloseRegion);
			}
		}
	}
}