using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	[dbc.ContractClass(typeof(IRouterContract))]
	internal interface IRouter
	{
		Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestNavigate(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);

		Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestClose(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRouter))]
	internal abstract class IRouterContract : IRouter
	{
		public Task RequestNavigate<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters is null");

			return default(Task);
		}

		public Task RequestNavigate(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters is null");

			return default(Task);
		}

		public Task RequestClose<TViewModel>(IRegion region, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters is null");

			return default(Task);
		}

		public Task RequestClose(IRegion region, ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region is null");
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters is null");

			return default(Task);
		}
	}

#pragma warning restore
}