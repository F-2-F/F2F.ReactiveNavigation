using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Navigator : INavigate
	{
		private readonly IRegion _region;
		private readonly IRouter _router;

		public Navigator(IRegion region, IRouter router)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");

			_region = region;
			_router = router;
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