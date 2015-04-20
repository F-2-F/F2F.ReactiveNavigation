using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.WPF
{
	// not needed, if we let the target control implement IRegion, but the adapter would make it reusable
	public interface IRegionAdapter<TRegionTarget> : IRegionAdapter
	{
	}
}