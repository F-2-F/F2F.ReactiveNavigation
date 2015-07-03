using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.Internal
{
	/// <summary>
	/// A multi items region caches all view models that are added until they get explicitly removed.
	/// If you navigate to a view model that already exists in this region, it is reused and activated.
	/// </summary>
	internal class MultiItemsRegion : Region
	{
		public MultiItemsRegion(ICreateViewModel viewModelFactory)
			: base(viewModelFactory)
		{
		}
	}
}
