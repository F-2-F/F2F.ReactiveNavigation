using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using F2F.ReactiveNavigation.ViewModel;
using System.Collections.Concurrent;

namespace F2F.ReactiveNavigation.WPF
{
	public class CachingContentRegionAdapter : RegionAdapter<ContentControl>
	{
		private readonly ICreateView _viewFactory;

		private ConcurrentDictionary<Type, object> _cachedViews = new ConcurrentDictionary<Type, object>();

		public CachingContentRegionAdapter(ContentControl regionTarget, ICreateView viewFactory)
			: base(regionTarget)
		{
			if (viewFactory == null)
				throw new ArgumentNullException("viewFactory", "viewFactory is null.");

			_viewFactory = viewFactory;
		}

		protected internal override void AddView(INavigableRegion region, ReactiveViewModel viewModel)
		{
			if (RegionTarget.Content != null)
				throw new InvalidOperationException("Content region target is not empty. Cannot add view in non-empty region.");

			var view = _cachedViews.GetOrAdd(viewModel.GetType(), _viewFactory.CreateViewFor(viewModel));

			RegionTarget.Content = view;
		}

		protected internal override void RemoveView(INavigableRegion region, ReactiveViewModel viewModel)
		{
			RegionTarget.Content = null;
		}

		protected internal override void ActivateView(INavigableRegion region, ReactiveViewModel viewModel)
		{
			// ??
		}

		protected internal override void DeactivateView(INavigableRegion region, ReactiveViewModel viewModel)
		{
			// ??
		}
	}
}
