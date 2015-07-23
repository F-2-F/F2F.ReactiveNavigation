using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System.Threading;

namespace F2F.ReactiveNavigation.Internal
{
	internal class Router : IRouter
	{
		public Router()
		{
		}

		public Task RequestNavigateAsync<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return RequestNavigateAsyncInternal<TViewModel>(region, parameters);
		}

		private async Task RequestNavigateAsyncInternal<TViewModel>(IRegion region, INavigationParameters parameters)
				where TViewModel : ReactiveViewModel
		{
			var target = await FindNavigationTarget<TViewModel>(region, parameters).ConfigureAwait(false);
			if (target != null)
			{
				await NavigateToExistingTarget(region, target, parameters).ConfigureAwait(false);
			}
			else
			{
				await NavigateToNewTarget<TViewModel>(region, parameters).ConfigureAwait(false);
			}
		}

		public Task RequestNavigateAsync(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");
			if (!region.Contains(navigationTarget))
				throw new ArgumentException("navigationTarget does not belong to region");

			return RequestNavigateAsyncInternal(region, navigationTarget, parameters);
		}

		private async Task RequestNavigateAsyncInternal(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			var canNavigateTo = await Observable.Start(() => navigationTarget.CanNavigateTo(parameters), RxApp.MainThreadScheduler).ToTask().ConfigureAwait(false);
			if (canNavigateTo)
			{
				await NavigateToExistingTarget(region, navigationTarget, parameters).ConfigureAwait(false);
			}

			await Task.FromResult(false);
		}

		public Task RequestCloseAsync<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");

			return RequestCloseAsyncInternal<TViewModel>(region, parameters);
		}

		private async Task RequestCloseAsyncInternal<TViewModel>(IRegion region, INavigationParameters parameters)
				where TViewModel : ReactiveViewModel
		{
			var target = await FindCloseTarget<TViewModel>(region, parameters).ConfigureAwait(false);
			if (target != null)
			{
				await CloseExistingTarget(region, target, parameters).ConfigureAwait(false);
			}

			await Task.FromResult(false);
		}

		public Task RequestCloseAsync(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");
			if (navigationTarget == null)
				throw new ArgumentNullException("navigationTarget", "navigationTarget is null.");
			if (parameters == null)
				throw new ArgumentNullException("parameters", "parameters is null.");
			if (!region.Contains(navigationTarget))
				throw new ArgumentException("navigationTarget does not belong to region");

			return RequestCloseAsyncInternal(region, navigationTarget, parameters);
		}

		private async Task RequestCloseAsyncInternal(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			var canClose = await Observable.Start(() => navigationTarget.CanClose(parameters), RxApp.MainThreadScheduler).ToTask().ConfigureAwait(false);
			if (canClose)
			{
				await CloseExistingTarget(region, navigationTarget, parameters).ConfigureAwait(false);
			}

			await Task.FromResult(false);
		}

		private static Task<TViewModel> FindNavigationTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return Observable.Start(() => region.Find<TViewModel>(vm => vm.CanNavigateTo(parameters)).FirstOrDefault(), RxApp.MainThreadScheduler).ToTask();
		}

		private async Task NavigateToExistingTarget(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			await Observable.Start(() => region.Activate(navigationTarget), RxApp.MainThreadScheduler).ToTask().ConfigureAwait(false);

			await Observable.Start(() => navigationTarget.NavigateTo(parameters), RxApp.TaskpoolScheduler).ToTask().ConfigureAwait(false);
		}

		private async Task NavigateToNewTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			// add view model and activate it in region, so...
			var navigationTarget = await AddViewModelTo<TViewModel>(region).ConfigureAwait(false);

			// ... that async initialization can get visualized (if visualization in place) ...
			//await Observable.Start(() => Observable.Defer(() => navigationTarget.InitializeAsync().ToObservable()), RxApp.TaskpoolScheduler).Concat();
			await navigationTarget.InitializeAsync().ConfigureAwait(false);

			await Observable.Start(() => region.Initialize(navigationTarget), RxApp.MainThreadScheduler).ToTask().ConfigureAwait(false);

			await Observable.Start(() => navigationTarget.NavigateTo(parameters), RxApp.TaskpoolScheduler).ToTask().ConfigureAwait(false);
		}

		private static Task<TViewModel> AddViewModelTo<TViewModel>(IRegion region)
				where TViewModel : ReactiveViewModel
		{
			return Observable.Start(() =>
			{
				var navigationTarget = region.Add<TViewModel>();

				region.Activate(navigationTarget);

				return navigationTarget;
			}, RxApp.MainThreadScheduler).ToTask();
		}

		private static Task<TViewModel> FindCloseTarget<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			return Observable.Start(() => region.Find<TViewModel>(vm => vm.CanClose(parameters)).FirstOrDefault(), RxApp.MainThreadScheduler).ToTask();
		}

		private async Task CloseExistingTarget(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			await Observable.Start(() => navigationTarget.Close(parameters), RxApp.TaskpoolScheduler).ToTask().ConfigureAwait(false);

			await Observable.Start(() =>
			{
				region.Deactivate(navigationTarget);
				region.Remove(navigationTarget);
			}, RxApp.MainThreadScheduler).ToTask().ConfigureAwait(false);
		}
	}
}