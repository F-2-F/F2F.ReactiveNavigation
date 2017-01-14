using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using F2F.ReactiveNavigation.ViewModel;
using System.Collections.Concurrent;

namespace F2F.ReactiveNavigation.WPF
{
    public class ContentRegionAdapter : RegionAdapter<ContentControl>
    {
        private readonly ICreateView _viewFactory;

        private readonly ConcurrentDictionary<ReactiveViewModel, object> _cachedViews 
            = new ConcurrentDictionary<ReactiveViewModel, object>();

        public ContentRegionAdapter(ContentControl regionTarget, ICreateView viewFactory)
            : base(regionTarget)
        {
            if (viewFactory == null)
                throw new ArgumentNullException("viewFactory", "viewFactory is null.");

            _viewFactory = viewFactory;
        }

        protected internal override void AddView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var view = _cachedViews.GetOrAdd(viewModel, _viewFactory.CreateViewFor(viewModel));

            RegionTarget.Content = view;
        }

        protected internal override void RemoveView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            RegionTarget.Content = null;

            object view;
            if (_cachedViews.TryRemove(viewModel, out view))
            {
                var disposable = view as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        protected internal override void ActivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            object view;
            if (_cachedViews.TryGetValue(viewModel, out view))
            {
                RegionTarget.Content = view;
            }
        }

        protected internal override void DeactivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
        }

        protected internal override void InitializeView(INavigableRegion region, ReactiveViewModel viewModel)
        {
        }
    }
}
