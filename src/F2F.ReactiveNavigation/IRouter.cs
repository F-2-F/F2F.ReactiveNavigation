using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(IRouterContract))]
	public interface IRouter
	{
		IRegion AddRegion(string name);

		[dbc.Pure]
		bool ContainsRegion(string regionName);

		Task RequestNavigate<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestClose<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRouter))]
	internal abstract class IRouterContract : IRouter
	{
		public IRegion AddRegion(string name)
		{
			dbc.Contract.Requires<ArgumentNullException>(name != null, "name is null");

			return default(IRegion);
		}

		public bool ContainsRegion(string regionName)
		{
			dbc.Contract.Requires<ArgumentNullException>(regionName != null, "regionName must not be null");

			return default(bool);
		}

		public Task RequestNavigate<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(regionName != null, "regionName must not be null");
			dbc.Contract.Requires<ArgumentException>(ContainsRegion(regionName), "unknown region name");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}

		public Task RequestClose<TViewModel>(string regionName, INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(regionName != null, "regionName must not be null");
			dbc.Contract.Requires<ArgumentException>(ContainsRegion(regionName), "unknown region name");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}
	}

#pragma warning restore
}