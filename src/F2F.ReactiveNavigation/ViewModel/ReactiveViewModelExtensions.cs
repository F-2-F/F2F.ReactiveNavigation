using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	// TODO: The filter parameter is redundant to the CanNavigateTo method.
	// The CanNavigateTo method is quite handy to use in the router, the filter Func is handy in the vm implementation.
	// If we use only the filter Func, the router wouldn't know which vm could effectively be navigated to, so we would need a different mechanism
	// to communicate this. Therefore I think, we should leave the CanNavigateTo method as long as there is no better idea.
	public static class ReactiveViewModelExtensions
	{
		public static IObservable<INavigationParameters> ObservableForNavigatedTo(this ReactiveViewModel This)
		{
			return This.NavigatedTo;
		}

		public static INavigationObservable<INavigationParameters> WhenNavigatedTo(this ReactiveViewModel This)
		{
			return new NavigationObservable<INavigationParameters>(This, This.NavigatedTo);
		}

		public static IObservable<INavigationParameters> ObservableForClosed(this ReactiveViewModel This)
		{
			return This.Closed;
		}

		public static INavigationObservable<INavigationParameters> WhenClosed(this ReactiveViewModel This)
		{
			return new NavigationObservable<INavigationParameters>(This, This.Closed);
		}
	}
}