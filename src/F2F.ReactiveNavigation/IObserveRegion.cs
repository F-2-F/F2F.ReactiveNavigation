using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
	public interface IObserveRegion
	{
		IObservable<ReactiveViewModel> Added { get; }

		IObservable<ReactiveViewModel> Removed { get; }

		IObservable<ReactiveViewModel> Activated { get; }

		IObservable<ReactiveViewModel> Deactivated { get; }

		IObservable<ReactiveViewModel> Initialized { get; }
	}
}