using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Internal
{
	internal interface IRegion
	{
		IEnumerable<ReactiveViewModel> ViewModels { get; }

		TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel;

		void Remove(ReactiveViewModel viewModel);

		void Activate(ReactiveViewModel viewModel);

		bool Contains(ReactiveViewModel viewModel);

		IEnumerable<TViewModel> Find<TViewModel>(Func<TViewModel, bool> predicate)
			where TViewModel : ReactiveViewModel;
	}
}