using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	public interface ICreateViewModel
	{
		ScopedLifetime<TViewModel> CreateViewModel<TViewModel>()
			where TViewModel : ReactiveViewModel;
	}
}