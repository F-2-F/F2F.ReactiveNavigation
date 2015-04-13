using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	[dbc.ContractClass(typeof(IRegionContract))]
	internal interface IRegion
	{
		TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel;

		void Remove(ReactiveViewModel viewModel);

		void Activate(ReactiveViewModel viewModel);

		[dbc.Pure]
		bool Contains(ReactiveViewModel viewModel);

		IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRegion))]
	internal abstract class IRegionContract : IRegion
	{
		public TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel
		{
			return default(TViewModel);
		}

		public void Remove(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");
			dbc.Contract.Requires<ArgumentException>(Contains(viewModel), "View model cannot be removed, since it is not contained in this region");
		}

		public void Activate(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");
			dbc.Contract.Requires<ArgumentException>(Contains(viewModel), "View model cannot be activated, since it is not contained in this region");
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			return default(bool);
		}

		public IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate)
		{
			dbc.Contract.Requires<ArgumentNullException>(predicate != null, "predicate is null");

			return default(IEnumerable<ReactiveViewModel>);
		}
	}

#pragma warning restore
}