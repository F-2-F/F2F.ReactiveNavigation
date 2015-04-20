using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.OwnedInstances;
using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF;
using F2F.ReactiveNavigation.WPF.Sample.Controller;
using F2F.ReactiveNavigation.WPF.Sample.ViewModel;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF.Sample
{
	internal class Bootstrapper
	{
		private class AutofacViewModelFactory : ICreateViewModel
		{
			private readonly IIndex<Type, Func<Owned<ReactiveViewModel>>> _factories;

			public AutofacViewModelFactory(IIndex<Type, Func<Owned<ReactiveViewModel>>> factories)
			{
				_factories = factories;
			}

			public ScopedLifetime<TViewModel> CreateViewModel<TViewModel>()
				where TViewModel : ReactiveViewModel
			{
				var factory = _factories[typeof(TViewModel)];

				var ownedViewModel = factory();

				return ((TViewModel)ownedViewModel.Value).Lifetime().EndingWith(ownedViewModel);
			}
		}

		private static int _viewModelCount = 0;

		public void Run()
		{
			var builder = new ContainerBuilder();

			builder
				.RegisterType<ViewFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder
				.RegisterType<AutofacViewModelFactory>()
				.AsImplementedInterfaces();

			builder
				.RegisterType<DummyDisposable>()
				.AsSelf();

			builder
				.RegisterType<SampleController>()
				.AsImplementedInterfaces();

			builder
				.Register(c => new RegionContainer(c.Resolve<ICreateViewModel>(), RxApp.MainThreadScheduler))
				.SingleInstance();

			builder
				.RegisterAssemblyTypes(typeof(Bootstrapper).Assembly)
				.Where(t => typeof(IBootstrapper).IsAssignableFrom(t))
				.As<IBootstrapper>();

			builder
				.RegisterAssemblyTypes(typeof(Bootstrapper).Assembly)
				.Where(t => typeof(ReactiveViewModel).IsAssignableFrom(t))
				.Keyed<ReactiveViewModel>(t => t);

			var container = builder.Build();

			var shellBuilder = new ContainerBuilder();

			var shell = InitializeShell(container, shellBuilder);
			shellBuilder.Update(container);

			// maybe use an autofac type?!
			var initializers = container.Resolve<IEnumerable<IBootstrapper>>();
			initializers.ToList().ForEach(i => i.Initialize());

			Application.Current.MainWindow = shell;
			shell.Show();
		}

		private Window InitializeShell(IContainer container, ContainerBuilder shellBuilder)
		{
			var shell = new MainWindow();

			var menuBuilder = new MenuBuilder(shell.MenuRegion);
			var regionContainer = container.Resolve<RegionContainer>();
			var tabRegion = regionContainer.CreateRegion();

			menuBuilder.AddMenuItem("Add", () => AddNewView(tabRegion));

			shellBuilder.RegisterInstance<IMenuBuilder>(menuBuilder);

			var viewFactory = container.Resolve<ICreateView>();
			var tabRegionAdapter = new TabRegionAdapter(viewFactory, shell.TabRegion);
			regionContainer.AdaptRegion(tabRegion, tabRegionAdapter);

			return shell;
		}

		private static void AddNewView(INavigableRegion tabRegion)
		{
			var naviParams = NavigationParameters.Create()
				.Add("value", _viewModelCount++);
			tabRegion.RequestNavigate<SampleViewModel>(naviParams);
		}
	}
}