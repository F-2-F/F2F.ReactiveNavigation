using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal class AdaptableRegion : IAdaptableRegion, IDisposable
	{
		private readonly NavigableRegion _region;

		private IList<IScopedLifetime<IRegionAdapter>> _regionAdapters = new List<IScopedLifetime<IRegionAdapter>>();

		public AdaptableRegion(NavigableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			_region = region;
		}

		public NavigableRegion Region
		{
			get { return _region; }
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
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return Region.RequestNavigate<TViewModel>(parameters);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return Region.RequestNavigate(navigationTarget, parameters);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return Region.RequestClose<TViewModel>(parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return Region.RequestClose(navigationTarget, parameters);
		}

		public void Adapt(IScopedLifetime<IRegionAdapter> regionAdapter)
		{
			if (regionAdapter == null)
				throw new ArgumentNullException("regionAdapter", "regionAdapter is null.");

			regionAdapter.Object.Adapt(_region);

			_regionAdapters.Add(regionAdapter);
		}

		public void Dispose()
		{
			if (_regionAdapters != null)
			{
				foreach (var r in _regionAdapters)
				{
					r.Dispose();
				}

				_regionAdapters = null;
			}
		}
	}
}