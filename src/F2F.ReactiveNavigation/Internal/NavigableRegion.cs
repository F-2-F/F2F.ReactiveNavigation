using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal class NavigableRegion : INavigableRegion
	{
		private readonly Region _region;
		private readonly IRouter _router;

		public NavigableRegion(Region region, IRouter router)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (router == null)
				throw new ArgumentNullException("router", "router is null.");

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
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return _router.RequestNavigate<TViewModel>(_region, parameters);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return _router.RequestNavigate(_region, navigationTarget, parameters);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return _router.RequestClose<TViewModel>(_region, parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return _router.RequestClose(_region, navigationTarget, parameters);
		}

		public async Task CloseAll()
		{
			foreach (var vm in _region.Find(_ => true))
			{
				await _router.RequestClose(Region, vm, NavigationParameters.CloseRegion);
			}
		}
	}
}