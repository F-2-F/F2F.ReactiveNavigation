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

        internal Region Region
        {
            get { return _region; }
        }

        internal IRouter Router
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

        public IObservable<ReactiveViewModel> Deactivated
        {
            get { return Region.Deactivated; }
        }

        public IObservable<ReactiveViewModel> Initialized
        {
            get { return Region.Initialized; }
        }

        public async Task RequestNavigate<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            await Router.RequestNavigateAsync<TViewModel>(Region, parameters).ConfigureAwait(false);
        }

        public async Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            if (navigationTarget == null)
                throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            await Router.RequestNavigateAsync(Region, navigationTarget, parameters).ConfigureAwait(false);
        }

        public async Task RequestClose<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            await Router.RequestCloseAsync<TViewModel>(Region, parameters);
        }

        public async Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            if (navigationTarget == null)
                throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            await Router.RequestCloseAsync(Region, navigationTarget, parameters).ConfigureAwait(false);
        }

        public async Task CloseAll()
        {
            foreach (var vm in Region.ViewModels)
            {
                await Router.RequestCloseAsync(Region, vm, NavigationParameters.CloseRegion).ConfigureAwait(false);
            }
        }
    }
}