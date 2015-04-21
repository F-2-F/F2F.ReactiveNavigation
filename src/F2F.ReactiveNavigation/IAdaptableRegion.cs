using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation
{
	public interface IAdaptableRegion : INavigableRegion
	{
		void Adapt(IScopedLifetime<IRegionAdapter> regionAdapter);
	}
}