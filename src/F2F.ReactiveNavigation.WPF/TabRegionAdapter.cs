using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.WPF
{
	public class TabRegionAdapter : IRegionAdapter<TabControl>, IDisposable
	{
		private readonly ICreateView _viewFactory;
		private readonly IRegion _region;
		private readonly TabControl _regionTarget;

		private bool _suppressSelectionChanged;
		private CompositeDisposable _disposables = new CompositeDisposable();
		private IDictionary<ReactiveViewModel, TabItem> _viewContainer = new Dictionary<ReactiveViewModel, TabItem>();

		public TabRegionAdapter(ICreateView viewFactory, IRegion region, TabControl regionTarget)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewFactory != null, "viewFactory must not be null");
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");
			dbc.Contract.Requires<ArgumentNullException>(regionTarget != null, "regionTarget must not be null");

			_viewFactory = viewFactory;
			_region = region;
			_regionTarget = regionTarget;
		}

		public void Adapt()
		{
			_disposables.Add(_region.Added.Do(vm => AddViewFor(vm)).Subscribe());
			_disposables.Add(_region.Removed.Do(vm => RemoveViewFor(vm)).Subscribe());
			_disposables.Add(_region.Activated.Do(vm => ActivateViewFor(vm)).Subscribe());

			_disposables.Add(
				Observable
					.FromEventPattern<SelectionChangedEventArgs>(_regionTarget, "SelectionChanged")
					.Where(e => ReferenceEquals(e.EventArgs.OriginalSource, _regionTarget))
					.Where(_ => !_suppressSelectionChanged)
					.Where(_ => _regionTarget.SelectedItem != null)
					.Do(async _ => await _region.RequestNavigate(LookupViewModel(_regionTarget.SelectedItem), NavigationParameters.UserNavigation()))	// TODO: nav parameters ?!?!
					.Subscribe());
		}

		private void AddViewFor(ReactiveViewModel viewModel)
		{
			var view = _viewFactory.CreateViewFor(viewModel);

			var tabViewModel = new TabViewModel(_region, viewModel);
			tabViewModel.InitializeAsync().Wait();

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