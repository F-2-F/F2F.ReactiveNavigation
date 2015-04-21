using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public class RegionContainer : IRegionContainer, IDisposable
	{
		private readonly Internal.Router _router;
		private readonly ICreateViewModel _viewModelFactory;

		private IList<IScopedLifetime<Internal.AdaptableRegion>> _regions = new List<IScopedLifetime<Internal.AdaptableRegion>>(); // TODO use Concurrent collection

		public RegionContainer(ICreateViewModel viewModelFactory, IScheduler routingScheduler)
		{
			if (viewModelFactory == null)
				throw new ArgumentNullException("viewModelFactory", "viewModelFactory is null.");
			if (routingScheduler == null)
				throw new ArgumentNullException("routingScheduler", "routingScheduler is null.");

			_viewModelFactory = viewModelFactory;
			_router = new Internal.Router(routingScheduler);
		}

		public IAdaptableRegion CreateRegion()
		{
			var region = new Internal.Region(_router, _viewModelFactory);
			var navRegion = new Internal.NavigableRegion(region, _router);
			var adaptRegion = new Internal.AdaptableRegion(navRegion);
			var scope = Scope.From(adaptRegion, adaptRegion, region);

			_regions.Add(scope);

			return adaptRegion;
		}

		public bool ContainsRegion(IAdaptableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			return _regions.Any(r => r.Object == region as Internal.AdaptableRegion);
		}

		public async Task RemoveRegion(IAdaptableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			var adaptRegion = region as Internal.AdaptableRegion;

			if (adaptRegion == null)
				throw new ArgumentException("given region is no instance of AdaptableRegion");

			var scope = _regions.FirstOrDefault(r => r.Object == region);

			if (scope == null)
				throw new ArgumentException("given region is not contained in RegionContainer");

			await scope.Object.Region.CloseAll();

			_regions.Remove(scope);

			scope.Dispose();
		}

		//public void AdaptRegion<TView>(INavigableRegion region)
		//{
		//	ICreateRegionAdapter moep = null;
		//	var regionAdapter = moep.CreateRegionAdapter<TView>();
		//	regionAdapter.Adapt(region);
		//}

		public void Dispose()
		{
			if (_regions != null)
			{
				foreach (var r in _regions)
				{
					r.Dispose();
				}

				_regions = null;
			}
		}
	}
}