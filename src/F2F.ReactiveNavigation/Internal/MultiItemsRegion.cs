using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.Internal
{
	internal class MultiItemsRegion : Region
	{
		public MultiItemsRegion(ICreateViewModel viewModelFactory)
			: base(viewModelFactory)
		{
		}
	}
}
