using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(INavigateContract))]
	public interface INavigate
	{
		Task RequestNavigate<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters);

		Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel;

		Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(INavigate))]
	internal abstract class INavigateContract : INavigate
	{
		public Task RequestNavigate<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}

		public Task RequestClose<TViewModel>(INavigationParameters parameters)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");

			return default(Task);
		}
	}

#pragma warning restore
}