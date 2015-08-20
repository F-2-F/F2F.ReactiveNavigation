using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public class RegionContainer : IRegionContainer, IDisposable
	{
		private readonly ICreateViewModel _viewModelFactory;
		private readonly Internal.Router _router = new Internal.Router();

		private ConcurrentDictionary<string, IScopedLifetime<Internal.AdaptableRegion>> _regions
			= new ConcurrentDictionary<string, IScopedLifetime<Internal.AdaptableRegion>>();

		// TODO shall viewModelFactory be of IScopedLifetime?
		public RegionContainer(ICreateViewModel viewModelFactory)
		{
			if (viewModelFactory == null)
				throw new ArgumentNullException("viewModelFactory", "viewModelFactory is null.");

			_viewModelFactory = viewModelFactory;
		}

		internal Internal.Router Router
		{
			get { return _router; }
		}

		public ICreateViewModel ViewModelFactory
		{
			get { return _viewModelFactory; }
		}

		public IAdaptableRegion CreateSingleItemRegion(string regionName)
		{
			return CreateAdaptableRegionFrom(regionName, new Internal.SingleItemRegion(ViewModelFactory));
		}

		public IAdaptableRegion CreateMultiItemsRegion(string regionName)
		{
			return CreateAdaptableRegionFrom(regionName, new Internal.MultiItemsRegion(ViewModelFactory));
		}

		private IAdaptableRegion CreateAdaptableRegionFrom(string regionName, Internal.Region region)
		{
			var navRegion = new Internal.NavigableRegion(region, Router);
			var adaptRegion = new Internal.AdaptableRegion(navRegion);
			var scope = Scope.From(adaptRegion, adaptRegion, region);

			if (!_regions.TryAdd(regionName, scope))
			{
				throw new ArgumentException("given region has already been added to this container");
			}

			return adaptRegion;
		}

		public bool ContainsRegion(IAdaptableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			return _regions.Any(kvp => kvp.Value.Object == region as Internal.AdaptableRegion);
		}

		public bool ContainsRegion(string regionName)
		{
			if (regionName == null)
				throw new ArgumentNullException("regionName", "regionName is null.");

			return _regions.ContainsKey(regionName);
		}

		public IAdaptableRegion GetRegion(string regionName)
		{
			if (regionName == null)
				throw new ArgumentNullException("regionName", "regionName is null.");

			IScopedLifetime<Internal.AdaptableRegion> scope;
			if (!_regions.TryGetValue(regionName, out scope))
			{
				throw new ArgumentException("given region is not contained in container");
			}

			return scope.Object;
		}

		public async Task RemoveRegion(IAdaptableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			var adaptRegion = region as Internal.AdaptableRegion;

			if (adaptRegion == null)
				throw new ArgumentException("given region is no instance of AdaptableRegion");

			var regionName = _regions.FirstOrDefault(kvp => kvp.Value.Object == region).Key;

			if (regionName == null)
				throw new ArgumentException("given region is not contained in container");

			IScopedLifetime<Internal.AdaptableRegion> scope;
			if (_regions.TryRemove(regionName, out scope))
			{
				await scope.Object.NavigableRegion.CloseAll();

				scope.Dispose();
			}
		}

		public void Dispose()
		{
			if (_regions != null)
			{
				foreach (var r in _regions)
				{
					r.Value.Dispose();
				}

				_regions = null;
			}
		}
	}
}