using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class AdaptableRegion : IAdaptableRegion, IDisposable
	{
		private readonly NavigableRegion _region;

		private readonly IList<ScopedLifetime<IRegionAdapter>> _regionAdapters = new List<ScopedLifetime<IRegionAdapter>>();

		public AdaptableRegion(NavigableRegion region)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");

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
			return Region.RequestNavigate<TViewModel>(parameters);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return Region.RequestNavigate(navigationTarget, parameters);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return Region.RequestClose<TViewModel>(parameters);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return Region.RequestClose(navigationTarget, parameters);
		}

		public void Adapt(ScopedLifetime<IRegionAdapter> regionAdapter)
		{
			regionAdapter.Object.Adapt(_region);

			_regionAdapters.Add(regionAdapter);
		}

		public void Dispose()
		{
			_region.Dispose();
		}
	}
}