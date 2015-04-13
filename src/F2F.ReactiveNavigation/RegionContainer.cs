using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	public class RegionContainer : IDisposable
	{
		private readonly Internal.IRouter _router;
		private readonly ICreateViewModel _viewModelFactory;
		private readonly IList<Internal.Region> _regions = new List<Internal.Region>();

		public RegionContainer(ICreateViewModel viewModelFactory, IScheduler scheduler)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModelFactory != null, "viewModelFactory must not be null");
			dbc.Contract.Requires<ArgumentNullException>(scheduler != null, "scheduler must not be null");

			_router = new Internal.Router(scheduler);
			_viewModelFactory = viewModelFactory;
		}

		public IRegion CreateRegion()
		{
			var region = new Internal.Region(_router, _viewModelFactory);
			_regions.Add(region);

			return region;
		}

		public INavigate CreateNavigator(IRegion region)
		{
			var internalRegion = _regions.FirstOrDefault(r => r == region);

			return new Internal.Navigator(internalRegion, _router);
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