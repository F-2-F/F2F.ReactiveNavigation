using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public interface IRegionContainer
	{
		INavigableRegion CreateRegion();

		bool ContainsRegion(INavigableRegion region);

		Task RemoveRegion(INavigableRegion region);

		void AdaptRegion(INavigableRegion region, IRegionAdapter regionAdapter);
	}
}