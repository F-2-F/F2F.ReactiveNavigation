using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public interface IRegionContainer
	{
		IAdaptableRegion CreateSingleItemRegion(string regionName);

		IAdaptableRegion CreateMultiItemsRegion(string regionName);

		bool ContainsRegion(IAdaptableRegion region);

		bool ContainsRegion(string regionName);

		IAdaptableRegion GetRegion(string regionName);

		Task RemoveRegion(IAdaptableRegion region);
	}
}