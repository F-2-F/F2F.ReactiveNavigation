using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using F2F.ReactiveNavigation.ViewModel;
using System.Windows;

namespace F2F.ReactiveNavigation.WPF
{
	/// <summary>
	/// Adapts a region to a content control. Everytime a view model is added, a new view will be created.
	/// If you want a content region that caches views, use the <see cref="CachingContentRegionAdapter"/>.
	/// </summary>
	public class ContentRegionAdapter : RegionAdapter<ContentControl>
	{
		private readonly ICreateView _viewFactory;

		public ContentRegionAdapter(ContentControl regionTarget, ICreateView viewFactory)
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

			var view = _viewFactory.CreateViewFor(viewModel);

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