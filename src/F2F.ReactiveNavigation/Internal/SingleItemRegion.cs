using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.Internal
{
	internal class SingleItemRegion : Region
	{
		public SingleItemRegion(ICreateViewModel viewModelFactory)
			: base(viewModelFactory)
		{
		}

		protected internal override void Adding<TViewModel>(TViewModel itemToBeAdded)
		{
			if (ViewModels.Any())
			{
				Remove(ViewModels.Single());
			}
		}
	}
}