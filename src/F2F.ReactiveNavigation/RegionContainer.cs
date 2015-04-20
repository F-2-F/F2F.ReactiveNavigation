using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	public class RegionContainer : IRegionContainer, IDisposable
	{
		private readonly Internal.Router _router;
		private readonly ICreateViewModel _viewModelFactory;

		private readonly IList<Internal.Region> _regions = new List<Internal.Region>();
		private readonly IList<IRegionAdapter> _regionAdapters = new List<IRegionAdapter>();

		public RegionContainer(ICreateViewModel viewModelFactory, IScheduler routingScheduler)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModelFactory != null, "viewModelFactory must not be null");
			dbc.Contract.Requires<ArgumentNullException>(routingScheduler != null, "routingScheduler must not be null");

			_viewModelFactory = viewModelFactory;
			_router = new Internal.Router(routingScheduler);
		}

		public INavigableRegion CreateRegion()
		{
			var region = new Internal.Region(_router, _viewModelFactory);

			_regions.Add(region);

			return new Internal.NavigableRegion(region, _router);
		}

		public bool ContainsRegion(INavigableRegion region)
		{
			var internalRegion = region as Internal.NavigableRegion;

			return internalRegion != null ? _regions.Any(r => r == internalRegion.Region) : false;
		}

		public async Task RemoveRegion(INavigableRegion region)
		{
			if (!(region is Internal.NavigableRegion))
				throw new ArgumentException("given region is no instance of NavigableRegion");

			var internalRegion = region as Internal.NavigableRegion;

			await internalRegion.CloseAll(); // TODO shall we trigger an event on region named "RegionClosed"?

			_regions.Remove(internalRegion.Region);
		}

		public void AdaptRegion(INavigableRegion region, IRegionAdapter regionAdapter)
		{
			regionAdapter.Adapt(region);

			_regionAdapters.Add(regionAdapter); // TODO we shall add adapter to region's scope so we can dispose at RemoveRegion
		}

		public void Dispose()
		{
			foreach (var vm in _regions)
			{
				vm.Dispose();
			}
		}
	}
}