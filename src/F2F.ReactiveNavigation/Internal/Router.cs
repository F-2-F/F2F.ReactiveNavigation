using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Router : IRouter
	{
		private readonly IScheduler _scheduler;

		public Router(IScheduler scheduler)
		{
			if (scheduler == null)
				throw new ArgumentNullException("scheduler", "scheduler is null.");

			_scheduler = scheduler;
		}

		public async Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			var target = FindNavigationTarget<TViewModel>(region, parameters);
			if (target != null)
			{
				await NavigateToExistingTarget(region, target, parameters);
			}
			else
			{
				await NavigateToNewTarget<TViewModel>(region, parameters);
			}
		}

		public async Task RequestNavigate(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");
			if (!region.Contains(navigationTarget))
				throw new ArgumentException("navigationTarget does not belong to region");

			if (navigationTarget.CanNavigateTo(parameters))
			{
				await NavigateToExistingTarget(region, navigationTarget, parameters);
			}

			await Task.FromResult(false);
		}

		public async Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			var target = FindCloseTarget<TViewModel>(region, parameters);
			if (target != null)
			{
				await CloseExistingTarget(region, target, parameters);
			}

			await Task.FromResult(false);
		}

		public async Task RequestClose(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");
			if (!region.Contains(navigationTarget))
				throw new ArgumentException("navigationTarget does not belong to region");

			if (navigationTarget.CanClose(parameters))
			{
				await CloseExistingTarget(region, navigationTarget, parameters);
			}

			await Task.FromResult(false);
		}

		private static ReactiveViewModel FindNavigationTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return region.Find<TViewModel>(vm => vm.CanNavigateTo(parameters)).FirstOrDefault();
		}

		private Task NavigateToExistingTarget(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
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
			var navigationTarget = region.Add<TViewModel>();

			region.Activate(navigationTarget);

			// ... that async initialization can get visualized (if visualization in place)
			await navigationTarget.InitializeAsync();

			navigationTarget.NavigateTo(parameters);
		}

		private static ReactiveViewModel FindCloseTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return region.Find<TViewModel>(vm => vm.CanClose(parameters)).FirstOrDefault();
		}

		private Task CloseExistingTarget(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return Observable.Start(() =>
			{
				navigationTarget.Close(parameters);

				region.Remove(navigationTarget);
			}, _scheduler).ToTask();
		}
	}
}