using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation
{
	
	public static class Router
	{
		public static IRouter Create(ICreateViewModel viewModelFactory, IScheduler scheduler)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModelFactory != null, "viewModelFactory is null");

			return new Internal.Router(viewModelFactory, scheduler);
		}
	}
}