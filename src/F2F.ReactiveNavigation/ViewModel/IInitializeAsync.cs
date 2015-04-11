using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public interface IInitializeAsync
	{
		Task InitializeAsync();
	}
}