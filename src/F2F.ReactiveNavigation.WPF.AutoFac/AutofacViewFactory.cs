using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.ViewModel;
using Autofac.Features.Indexed;
using F2F.ReactiveNavigation;
using System.Windows;

namespace F2F.ReactiveNavigation.WPF.Autofac
{
	public class AutofacViewFactory : ICreateView
	{
		private readonly IIndex<Type, Func<FrameworkElement>> _viewFactories;

		public AutofacViewFactory(IIndex<Type, Func<FrameworkElement>> viewFactories)
		{
			if (viewFactories == null)
				throw new ArgumentNullException("viewFactories", "viewFactories is null.");
			
			_viewFactories = viewFactories;
		}

		public object CreateViewFor(ReactiveViewModel viewModel)
		{
			if (viewModel == null)
				throw new ArgumentNullException("viewModel", "viewModel is null.");
			
			var factory = _viewFactories[viewModel.GetType()];

			var view = factory();
			view.DataContext = viewModel;

			return view;
		}
	}
}
