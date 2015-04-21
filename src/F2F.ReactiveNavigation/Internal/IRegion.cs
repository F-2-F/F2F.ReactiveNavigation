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
		TViewModel Add<TViewModel>()
			where TViewModel : ReactiveViewModel;

		void Remove(ReactiveViewModel viewModel);

		void Activate(ReactiveViewModel viewModel);

		bool Contains(ReactiveViewModel viewModel);

		IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate);
	}
}