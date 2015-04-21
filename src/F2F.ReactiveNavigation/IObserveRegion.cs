using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
	public interface IObserveRegion // maybe we need different types of region (single view region and multi view region, ...)
	{
		IObservable<ReactiveViewModel> Added { get; }

		IObservable<ReactiveViewModel> Removed { get; }

		IObservable<ReactiveViewModel> Activated { get; }
	}
}