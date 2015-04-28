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
		Task RequestNavigateAsync<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestNavigateAsync(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);

		Task RequestCloseAsync<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestCloseAsync(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);
	}
}