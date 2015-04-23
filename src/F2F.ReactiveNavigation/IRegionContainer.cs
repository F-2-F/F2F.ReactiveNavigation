using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public interface IRegionContainer
	{
		IAdaptableRegion CreateSingleItemRegion();

		IAdaptableRegion CreateMultiItemsRegion();

		bool ContainsRegion(IAdaptableRegion region);

		Task RemoveRegion(IAdaptableRegion region);
	}
}