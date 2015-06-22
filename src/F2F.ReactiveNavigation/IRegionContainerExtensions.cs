using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation
{
	public static class IRegionContainerExtensions
	{
		public static IAdaptableRegion GetRegion<TRegion>(this IRegionContainer container)
		{
			return container.GetRegion(typeof(TRegion).ToString());
		}
	}
}
