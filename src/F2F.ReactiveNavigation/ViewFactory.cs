using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
	public class ViewFactory : IRegisterViewFactories, ICreateView
	{
		private readonly IDictionary<Type, Func<ReactiveViewModel, object>> _catalog = new Dictionary<Type, Func<ReactiveViewModel, object>>();

		public object CreateViewFor(ReactiveViewModel viewModel)
		{
			var key = viewModel.GetType();
			Func<ReactiveViewModel, object> createView;
			if (_catalog.TryGetValue(key, out createView))
			{
				return createView(viewModel);
			}

			throw new NotSupportedException();
		}

		public void Register<TViewModel>(Func<ReactiveViewModel, object> createView)
			where TViewModel : ReactiveViewModel
		{
			_catalog.Add(typeof(TViewModel), createView);
		}
	}
}