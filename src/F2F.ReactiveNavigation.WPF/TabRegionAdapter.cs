using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.WPF
{
	public class TabRegionAdapter : IRegionAdapter<TabControl>, IDisposable
	{
		private readonly ICreateView _viewFactory;
		private readonly TabControl _regionTarget;

		private bool _suppressSelectionChanged;
		private CompositeDisposable _disposables = new CompositeDisposable();
		private IDictionary<ReactiveViewModel, TabItem> _viewContainer = new Dictionary<ReactiveViewModel, TabItem>(); // TODO use ConcurrentDictionary ?

		public TabRegionAdapter(ICreateView viewFactory, TabControl regionTarget)
		{
			if (viewFactory == null)
				throw new ArgumentNullException("viewFactory", "viewFactory is null.");
			if (regionTarget == null)
				throw new ArgumentNullException("regionTarget", "regionTarget is null.");

			_viewFactory = viewFactory;
			_regionTarget = regionTarget;
		}

		public void Adapt(INavigableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			_disposables.Add(region.Added.Do(vm => AddViewFor(region, vm)).Subscribe());
			_disposables.Add(region.Removed.Do(vm => RemoveViewFor(vm)).Subscribe());
			_disposables.Add(region.Activated.Do(vm => ActivateViewFor(vm)).Subscribe());

			_disposables.Add(
				Observable
					.FromEventPattern<SelectionChangedEventArgs>(_regionTarget, "SelectionChanged")
					.Where(e => ReferenceEquals(e.EventArgs.OriginalSource, _regionTarget))
					.Where(_ => !_suppressSelectionChanged)
					.Where(_ => _regionTarget.SelectedItem != null)
					.Do(_ => region.RequestNavigate(LookupViewModel(_regionTarget.SelectedItem), NavigationParameters.UserNavigation))	// TODO: SelectMany?
					.Subscribe());
		}

		private void AddViewFor(INavigableRegion region, ReactiveViewModel viewModel)
		{
			var view = _viewFactory.CreateViewFor(viewModel);

			var tabViewModel = new TabViewModel(region, viewModel);

			var tabView = new TabView()
			{
				DataContext = tabViewModel
			};

			var tab = new TabItem
			{
				Header = tabView,
				Content = view
			};

			_viewContainer.Add(viewModel, tab);
			_regionTarget.Items.Add(tab);
		}

		private void RemoveViewFor(ReactiveViewModel viewModel)
		{
			var view = LookupView(viewModel);
			_regionTarget.Items.Remove(view);
			_viewContainer.Remove(viewModel);
		}

		private void ActivateViewFor(ReactiveViewModel viewModel)
		{
			var view = LookupView(viewModel);
			try
			{
				_suppressSelectionChanged = true;
				_regionTarget.SelectedItem = view;
			}
			finally
			{
				_suppressSelectionChanged = false;
			}
		}

		private object LookupView(ReactiveViewModel viewModel)
		{
			return _viewContainer[viewModel];
		}

		private ReactiveViewModel LookupViewModel(object view)
		{
			return _viewContainer.First(kvp => kvp.Value == view).Key;
		}

		public void Dispose()
		{
			if (_disposables != null)
			{
				_disposables.Dispose();
				_disposables = null;
			}

			if (_viewContainer != null)
			{
				_viewContainer.Clear();
				_viewContainer = null;
			}
		}
	}
}