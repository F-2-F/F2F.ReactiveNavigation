using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(IRegionContract))]
	public interface IRegion // maybe we need different types of region (single view region and multi view region, ...)
	{
		string Name { get; }

		IObservable<ReactiveViewModel> Added { get; }

		IObservable<ReactiveViewModel> Removed { get; }

		IObservable<ReactiveViewModel> Activated { get; }

		[dbc.Pure]
		bool Contains(ReactiveViewModel viewModel);

		Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters);

		void RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRegion))]
	internal abstract class IRegionContract : IRegion
	{
		public string Name
		{
			get { return default(string); }
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");

			return default(bool);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");
			dbc.Contract.Requires<ArgumentException>(Contains(navigationTarget), "View model cannot be navigated to, since it is not contained in this region");

			return default(Task);
		}

		public void RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			dbc.Contract.Requires<ArgumentNullException>(navigationTarget != null, "navigationTarget is null");
			dbc.Contract.Requires<ArgumentNullException>(parameters != null, "parameters must not be null");
			dbc.Contract.Requires<ArgumentException>(Contains(navigationTarget), "View model cannot be closed, since it is not contained in this region");
		}
	}

#pragma warning restore
}