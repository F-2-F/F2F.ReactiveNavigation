using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reactive.Linq;
using F2F.ReactiveNavigation.ViewModel;
using System.Windows.Controls.Primitives;

namespace F2F.ReactiveNavigation.WPF
{
	public abstract class RegionAdapter<TRegionTarget> : IRegionAdapter<TRegionTarget>, IDisposable
		where TRegionTarget : FrameworkElement
	{
		private readonly TRegionTarget _regionTarget;
		
		private CompositeDisposable _disposables = new CompositeDisposable();

		public RegionAdapter(TRegionTarget regionTarget)
		{
			if (regionTarget == null)
				throw new ArgumentNullException("regionTarget", "regionTarget is null.");

			_regionTarget = regionTarget;
		}

		internal protected void AddDisposable(IDisposable disposable)
		{
			_disposables.Add(disposable);
		}

		public void Adapt(INavigableRegion region)
		{
			if (region == null)
				throw new ArgumentNullException("region", "region is null.");

			AddDisposable(region.Added.Do(vm => SynchronizeAddView(region, vm)).Subscribe());
			AddDisposable(region.Removed.Do(vm => SynchronizeRemoveView(region, vm)).Subscribe());
			AddDisposable(region.Activated.Do(vm => SynchronizeActivateView(region, vm)).Subscribe());
			AddDisposable(region.Activated.Do(vm => SynchronizeDeactivateView(region, vm)).Subscribe());
			AddDisposable(region.Initialized.Do(vm => SynchronizeInitializeView(region, vm)).Subscribe());

			AdaptToRegionTarget(region);
		}

        private readonly object _syncRoot = new object();

        internal protected void SynchronizeAddView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            lock(_syncRoot)
            {               
                AddView(region, viewModel);
            }
        }

        internal protected void SynchronizeRemoveView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            lock (_syncRoot)
            {
                RemoveView(region, viewModel);
            }
        }

        internal protected void SynchronizeActivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            lock (_syncRoot)
            {
                ActivateView(region, viewModel);
            }
        }

        internal protected void SynchronizeDeactivateView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            lock (_syncRoot)
            {
                DeactivateView(region, viewModel);
            }
        }

        internal protected void SynchronizeInitializeView(INavigableRegion region, ReactiveViewModel viewModel)
        {
            lock (_syncRoot)
            {
                InitializeView(region, viewModel);
            }
        }

        internal protected abstract void AddView(INavigableRegion region, ReactiveViewModel viewModel);
		internal protected abstract void RemoveView(INavigableRegion region, ReactiveViewModel viewModel);
		internal protected abstract void ActivateView(INavigableRegion region, ReactiveViewModel viewModel);
		internal protected abstract void DeactivateView(INavigableRegion region, ReactiveViewModel viewModel);
		internal protected abstract void InitializeView(INavigableRegion region, ReactiveViewModel viewModel);

		internal protected virtual void AdaptToRegionTarget(INavigableRegion region)
		{
		}

		public void Dispose()
		{
			if (_disposables != null)
			{
				_disposables.Dispose();
				_disposables = null;
			}
		}

		public TRegionTarget RegionTarget
		{
			get { return _regionTarget; }
		}
	}
}
