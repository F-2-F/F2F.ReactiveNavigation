using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Autofac.Features.OwnedInstances;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Autofac
{
	public class AutofacViewModelFactory : ICreateViewModel
	{
		private readonly IIndex<Type, Func<Owned<ReactiveViewModel>>> _factories;

		public AutofacViewModelFactory(IIndex<Type, Func<Owned<ReactiveViewModel>>> factories)
		{
			_factories = factories;
		}

		public IScopedLifetime<TViewModel> CreateViewModel<TViewModel>()
			where TViewModel : ReactiveViewModel
		{
			var factory = _factories[typeof(TViewModel)];

			var ownedViewModel = factory();

			return ((TViewModel)ownedViewModel.Value).Lifetime().EndingWith(ownedViewModel);
		}
	}
}