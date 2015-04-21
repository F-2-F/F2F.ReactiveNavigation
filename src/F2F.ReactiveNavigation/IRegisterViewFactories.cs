using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(IRegisterViewModelContract))]
	public interface IRegisterViewFactories
	{
		void Register<TViewModel>(Func<ReactiveViewModel, object> createView)
			where TViewModel : ReactiveViewModel;
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRegisterViewFactories))]
	internal abstract class IRegisterViewModelContract : IRegisterViewFactories
	{
		public void Register<TViewModel>(Func<ReactiveViewModel, object> createView)
			where TViewModel : ReactiveViewModel
		{
			dbc.Contract.Requires<ArgumentNullException>(createView != null, "createView is null");
		}
	}

#pragma warning restore
}