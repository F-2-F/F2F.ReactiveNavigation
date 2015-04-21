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
using F2F.ReactiveNavigation.Autofac;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF;
using F2F.ReactiveNavigation.WPF.Sample.Controller;
using F2F.ReactiveNavigation.WPF.Sample.ViewModel;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF.Sample
{
	internal class Bootstrapper : AutofacBootstrapper
	{
		private static int _viewModelCount = 0;

		public override void Run()
		{
			base.Run();

			var builder = new ContainerBuilder();

			builder
				.RegisterType<DummyDisposable>()
				.AsSelf();

			builder
				.RegisterType<SampleController>()
				.AsImplementedInterfaces();

			RegisterBootstrapper(GetType().Assembly);
			RegisterViewModels(GetType().Assembly);

			var container = builder.Build();

			var shellBuilder = new ContainerBuilder();

			var shell = InitializeShell(container, shellBuilder);
			shellBuilder.Update(container);

			RunRegisteredBootstrappers();

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
			var ra = new TabRegionAdapter(viewFactory, shell.TabRegion);
			var tabRegionAdapter = new ScopedLifetime<IRegionAdapter>(ra, ra); // TODO WTF???
			//tabRegionAdapter = Scope.From(ra, ra);
			tabRegion.Adapt(tabRegionAdapter);

			return shell;
		}

		private static void AddNewView(IAdaptableRegion tabRegion)
		{
			var naviParams = NavigationParameters.Create()
				.Add("value", _viewModelCount++);
			tabRegion.RequestNavigate<SampleViewModel>(naviParams);
		}
	}
}