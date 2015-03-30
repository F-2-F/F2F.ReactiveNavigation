using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	
	[dbc.ContractClass(typeof(IRouterContract))]
	internal interface IRouter : ReactiveNavigation.IRouter
	{
		Task RequestNavigate(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters);

		Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestClose(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters);

		Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;
	}


#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRouter))]
	internal abstract class IRouterContract : IRouter
	{
		public ReactiveNavigation.IRegion AddRegion(string name)
		{
			return default(ReactiveNavigation.IRegion);
		}

		public bool ContainsRegion(string regionName)
		{
			return default(bool);
		}

		public Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(ContainsRegion(region.Name), "unknown region name");

			return default(Task);
		}

		public Task RequestNavigate<TViewModel>(string regionName, INavigationParameters parameters) 
			where TViewModel : ReactiveViewModel
		{
			return default(Task);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(ContainsRegion(region.Name), "unknown region name");

			return default(Task);
		}

		public Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(ContainsRegion(region.Name), "unknown region name");

			return default(Task);
		}

		public Task RequestClose<TViewModel>(string regionName, INavigationParameters parameters) 
			where TViewModel : ReactiveViewModel
		{
			return default(Task);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, IRegion region, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(ContainsRegion(region.Name), "unknown region name");

			return default(Task);
		}
	}

#pragma warning restore
}