using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal interface IRouter
	{
		Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestNavigate(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);

		Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestClose(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);
	}
}