using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Router : IRouter
	{
		private readonly ICreateViewModel _viewModelFactory;
		private readonly IScheduler _scheduler;
		private readonly IDictionary<string, IRegion> _regions = new Dictionary<string, IRegion>(); // maybe we need a concurrent dictionary here
		private readonly IDictionary<ReactiveViewModel, IDisposable> _lifetimeScopes = new Dictionary<ReactiveViewModel, IDisposable>(); // maybe we need a concurrent dictionary here

		public Router(ICreateViewModel viewModelFactory, IScheduler scheduler)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModelFactory != null, "viewModelFactory must not be null");
			dbc.Contract.Requires<ArgumentNullException>(scheduler != null, "scheduler must not be null");

			_viewModelFactory = viewModelFactory;
			_scheduler = scheduler;
		}

		public ReactiveNavigation.IRegion AddRegion(string name)
		{
			var region = new Region(name, this);
			_regions.Add(region.Name, region);
			return region;
		}

		public bool ContainsRegion(string regionName)
		{
			return _regions.ContainsKey(regionName);
		}

		private IRegion FindRegion(string regionName)
		{
			dbc.Contract.Requires<ArgumentNullException>(ContainsRegion(regionName), "unknown region name");

			return _regions[regionName];
		}

		public async Task RequestNavigate<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			await RequestNavigate<TViewModel>(FindRegion(regionName), parameters);
		}

		public async Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			var target = FindNavigationTarget<TViewModel>(region, parameters);
			if (target != null)
			{
				await NavigateToExistingTarget(target, region, parameters);
			}
			else
			{
				await NavigateToNewTarget<TViewModel>(region, parameters);
			}
		}

		public async Task RequestNavigate(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters)
		{
			await NavigateToExistingTarget(navigationTarget, region, parameters);
		}

		public void RequestClose<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			RequestClose<TViewModel>(FindRegion(regionName), parameters);
		}

		public void RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			var target = FindNavigationTarget<TViewModel>(region, parameters);
			if (target != null)
			{
				RequestClose(target, region, parameters);
			}
		}

		public void RequestClose(ReactiveViewModel viewModel, IRegion region, INavigationParameters parameters)
		{
			if (CanClose(viewModel, parameters))
			{
				CloseExistingTarget(viewModel, region);
			}
		}

		private ReactiveViewModel FindNavigationTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return region.Find(vm => vm is TViewModel && vm.CanNavigateTo(parameters)).FirstOrDefault();
		}

		private Task NavigateToExistingTarget(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters)
		{
			return Observable.Start(() =>
			{
				region.Activate(navigationTarget);
				navigationTarget.NavigateTo(parameters);
			}, _scheduler).ToTask();
		}

		private async Task NavigateToNewTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			var scopedTarget = _viewModelFactory.CreateViewModel<TViewModel>();

			AddLifetimeScope(scopedTarget);

			var navigationTarget = scopedTarget.Object;
			// first add the item to the region, so...
			region.Add(navigationTarget);
			region.Activate(navigationTarget);

			// ... that async initialization can get visualized (if visualization in place)
			await navigationTarget.InitializeAsync();
			navigationTarget.NavigateTo(parameters);
		}

		private void AddLifetimeScope<TViewModel>(ScopedLifetime<TViewModel> scope)
			where TViewModel : ReactiveViewModel
		{
			_lifetimeScopes.Add((ReactiveViewModel)scope.Object, scope.Scope);
		}

		private void EndLifetime(ReactiveViewModel viewModel)
		{
			var scope = _lifetimeScopes[viewModel];
			scope.Dispose();
		}

		private bool CanClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return _lifetimeScopes.ContainsKey(navigationTarget) && navigationTarget.CanClose(parameters);
		}

		private void CloseExistingTarget(ReactiveViewModel navigationTarget, IRegion region)
		{
			region.Remove(navigationTarget);
			EndLifetime(navigationTarget);
		}
	}
}