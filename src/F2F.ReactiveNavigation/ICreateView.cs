using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(ICreateViewContract))]
	public interface ICreateView
	{
		object CreateViewFor(ReactiveViewModel viewModel);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(ICreateView))]
	internal abstract class ICreateViewContract : ICreateView
	{
		public object CreateViewFor(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");

			return default(object);
		}
	}

#pragma warning restore
}