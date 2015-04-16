using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	[dbc.ContractClass(typeof(IObserveRegionContract))]
	public interface IObserveRegion // maybe we need different types of region (single view region and multi view region, ...)
	{
		IObservable<ReactiveViewModel> Added { get; }

		IObservable<ReactiveViewModel> Removed { get; }

		IObservable<ReactiveViewModel> Activated { get; }
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IObserveRegion))]
	internal abstract class IObserveRegionContract : IObserveRegion
	{
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
	}

#pragma warning restore
}