using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
	public interface ICreateView
	{
		object CreateViewFor(ReactiveViewModel viewModel);
	}
}