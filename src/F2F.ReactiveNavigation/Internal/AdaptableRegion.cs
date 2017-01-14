using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
    internal class AdaptableRegion : IAdaptableRegion, IDisposable
    {
        private readonly NavigableRegion _navigableRegion;

        private IList<IScopedLifetime<IRegionAdapter>> _regionAdapters = new List<IScopedLifetime<IRegionAdapter>>();

        public AdaptableRegion(NavigableRegion navigableRegion)
        {
            if (navigableRegion == null)
                throw new ArgumentNullException("navigableRegion", "navigableRegion is null.");

            _navigableRegion = navigableRegion;
        }

        public NavigableRegion NavigableRegion
        {
            get { return _navigableRegion; }
        }

        public IObservable<ReactiveViewModel> Added
        {
            get { return NavigableRegion.Added; }
        }

        public IObservable<ReactiveViewModel> Removed
        {
            get { return NavigableRegion.Removed; }
        }

        public IObservable<ReactiveViewModel> Activated
        {
            get { return NavigableRegion.Activated; }
        }

        public IObservable<ReactiveViewModel> Deactivated
        {
            get { return NavigableRegion.Activated; }
        }

        public IObservable<ReactiveViewModel> Initialized
        {
            get { return NavigableRegion.Initialized; }
        }

        public Task RequestNavigate<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            return NavigableRegion.RequestNavigate<TViewModel>(parameters);
        }
        
        public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            if (navigationTarget == null)
                throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            return NavigableRegion.RequestNavigate(navigationTarget, parameters);
        }

        public Task RequestClose<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            return NavigableRegion.RequestClose<TViewModel>(parameters);
        }

        public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            if (navigationTarget == null)
                throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            return NavigableRegion.RequestClose(navigationTarget, parameters);
        }

        public Task CloseAll()
        {
            return NavigableRegion.CloseAll();
        }

        public void Adapt(IScopedLifetime<IRegionAdapter> regionAdapter)
        {
            if (regionAdapter == null)
                throw new ArgumentNullException("regionAdapter", "regionAdapter is null.");

            regionAdapter.Object.Adapt(_navigableRegion);

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