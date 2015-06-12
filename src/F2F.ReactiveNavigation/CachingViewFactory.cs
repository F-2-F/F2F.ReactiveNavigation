using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.ViewModel;
using System.Collections.Concurrent;

namespace F2F.ReactiveNavigation
{
	// Maybe we can remove this class again
	public class CachingViewFactory : ICreateView
	{
		private readonly ICreateView _viewFactory;

		private ConcurrentDictionary<Type, object> _cachedViews = new ConcurrentDictionary<Type, object>();

		public CachingViewFactory(ICreateView viewFactory)
		{
			if (viewFactory == null)
				throw new ArgumentNullException("viewFactory", "viewFactory is null.");
			
			_viewFactory = viewFactory;
		}
		
		public object CreateViewFor(ReactiveViewModel viewModel)
		{
			return _cachedViews.GetOrAdd(viewModel.GetType(), _viewFactory.CreateViewFor(viewModel));
		}
	}
}