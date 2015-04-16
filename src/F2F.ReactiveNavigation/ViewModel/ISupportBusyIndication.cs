using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public interface ISupportBusyIndication
	{
		/// <summary>Gets a value indicating whether this object is busy.</summary>
		/// <value>true if this object is busy, false if not.</value>
		bool IsBusy { get; }
	}
}