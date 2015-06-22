using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
	// TODO: we should completely remove this interface as it is implementation detail of RegionAdapters
	public interface ICreateView
	{
		// TODO: Think of returning an "IView" that allows access to the 
		// view model the view is based on. object gives us nothing, and a simple IView
		// interface could be easily implemented in portable scenario.
		object CreateViewFor(ReactiveViewModel viewModel);
	}

}