using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace F2F.ReactiveNavigation.WPF
{
    /// <summary>
    /// RegionAdapter for Selector controls that can be used when working with data templates for
    /// view models. 
    /// </summary>
    /// <typeparam name="TSelector"></typeparam>
    public abstract class SelectorRegionAdapter<TSelector> : RegionAdapter<TSelector>
        where TSelector : Selector
    {
        private bool _suppressSelectionChanged;

        public SelectorRegionAdapter(TSelector regionTarget)
            : base(regionTarget)
        {
        }

        protected internal override void AdaptToRegionTarget(INavigableRegion region)
        {
            AddDisposable(
                Observable
                    .FromEventPattern<SelectionChangedEventArgs>(RegionTarget, "SelectionChanged")
                    .Where(e => ReferenceEquals(e.EventArgs.OriginalSource, RegionTarget))
                    .Where(_ => !_suppressSelectionChanged)
                    .Where(_ => RegionTarget.SelectedItem != null)
                    .Do(_ => region.RequestNavigate(RegionTarget.SelectedItem as ReactiveViewModel, NavigationParameters.UserNavigation))    // TODO: SelectMany?
                    .Subscribe());
        }
        
        protected internal override void AddView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var items = RegionTarget.ItemsSource as IList ?? RegionTarget.Items as IList;

            if (items != null)
            {
                var sortableItem = viewModel as ISortable;
                if (sortableItem != null)
                {
                    var insertAtIndex = items.Count -
                        items
                            .OfType<FrameworkElement>()
                            .Select(x => x.DataContext)
                            .Cast<ISortable>()
                            .OrderBy(x => x.SortHint)
                            .SkipWhile(x => x.SortHint < sortableItem.SortHint)
                            .Count();

                    items.Insert(insertAtIndex, sortableItem);
                }
                else
                {
                    items.Add(viewModel);
                }
            }
        }

        protected internal override void RemoveView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var items = RegionTarget.ItemsSource as IList ?? RegionTarget.Items as IList;
            if (items.Contains(viewModel))
            {
                items.Remove(viewModel);
            }
        }

        protected internal override void ActivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            try
            {
                _suppressSelectionChanged = true;
                RegionTarget.SelectedItem = viewModel;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        protected internal override void DeactivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            // no use case yet
        }
    }

    /// <summary>
    /// Region adapter for Selector controls that can be used when explicit view creation is required per view model.
    /// This is the choice when not working with data templates, but with the view factory.
    /// </summary>
    /// <typeparam name="TSelector"></typeparam>
    /// <typeparam name="TViewHost"></typeparam>
    public abstract class SelectorRegionAdapter<TSelector, TViewHost> : RegionAdapter<TSelector>
            where TSelector : Selector
            where TViewHost : FrameworkElement
    {
        private readonly ICreateView _viewFactory;
        private bool _suppressSelectionChanged;
        private ConcurrentDictionary<ReactiveViewModel, TViewHost> _viewContainer = new ConcurrentDictionary<ReactiveViewModel, TViewHost>(); 

        public SelectorRegionAdapter(ICreateView viewFactory, TSelector regionTarget)
            : base(regionTarget)
        {
            if (viewFactory == null)
                throw new ArgumentNullException("viewFactory", "viewFactory is null.");

            _viewFactory = viewFactory;
        }

        public ICreateView ViewFactory
        {
            get { return _viewFactory; }
        }

        protected internal override void AdaptToRegionTarget(INavigableRegion region)
        {
            AddDisposable(
                Observable
                    .FromEventPattern<SelectionChangedEventArgs>(RegionTarget, "SelectionChanged")
                    .Where(e => ReferenceEquals(e.EventArgs.OriginalSource, RegionTarget))
                    .Where(_ => !_suppressSelectionChanged)
                    .Where(_ => RegionTarget.SelectedItem != null)
                    .Do(_ => region.RequestNavigate(LookupViewModel(RegionTarget.SelectedItem), NavigationParameters.UserNavigation))    // TODO: SelectMany?
                    .Subscribe());
        }

        protected abstract TViewHost CreateViewHost(ReactiveViewModel viewModel, object view);

        protected internal override void AddView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var view = ViewFactory.CreateViewFor(viewModel);

            var viewHost = CreateViewHost(viewModel, view);

            _viewContainer.AddOrUpdate(viewModel, viewHost, (_, v) => v);

            var items = RegionTarget.ItemsSource as IList ?? RegionTarget.Items as IList;

            if (items != null)
            {
                var sortableItem = viewModel as ISortable;
                if (sortableItem != null)
                {
                    var insertAtIndex = items.Count -
                        items
                            .OfType<FrameworkElement>()
                            .Select(x => x.DataContext)
                            .Cast<ISortable>()
                            .OrderBy(x => x.SortHint)
                            .SkipWhile(x => x.SortHint < sortableItem.SortHint)
                            .Count();

                    items.Insert(insertAtIndex, viewHost);
                }
                else
                {
                    items.Add(viewHost);
                }
            }
        }

        protected internal override void RemoveView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var view = LookupView(viewModel);

            var items = RegionTarget.ItemsSource as IList ?? RegionTarget.Items as IList;
            if (items.Contains(view))
            {
                items.Remove(view);
            }
            
            TViewHost host;
            _viewContainer.TryRemove(viewModel, out host);
        }

        protected internal override void ActivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            var view = LookupView(viewModel);
            try
            {
                _suppressSelectionChanged = true;
                RegionTarget.SelectedItem = view;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        protected internal override void DeactivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            // no use case yet
        }

        private object LookupView(ReactiveViewModel viewModel)
        {
            return _viewContainer[viewModel];
        }

        private ReactiveViewModel LookupViewModel(object view)
        {
            return _viewContainer.First(kvp => kvp.Value == view).Key;
        }
    }
}
