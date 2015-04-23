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

			RegisterInitializers(GetType().Assembly);
			RegisterViewModels(GetType().Assembly);

			builder.Update(Container);

			var shell = InitializeShell();

			RunInitializers();

			Application.Current.MainWindow = shell;
			shell.Show();
		}

		private Window InitializeShell()
		{
			var shellBuilder = new ContainerBuilder();

			var shell = new MainWindow();

			var menuBuilder = new MenuBuilder(shell.MenuRegion);
			var regionContainer = Container.Resolve<IRegionContainer>();
			var tabRegion = regionContainer.CreateRegion();

			menuBuilder.AddMenuItem("Add", () => AddNewView(tabRegion));
			menuBuilder.AddMenuItem("Other", () => tabRegion.RequestNavigate<OtherViewModel>(NavigationParameters.UserNavigation));
			menuBuilder.AddMenuItem("Close all", () => tabRegion.CloseAll());

			shellBuilder.RegisterInstance<IMenuBuilder>(menuBuilder);

			var viewFactory = Container.Resolve<ICreateView>();
			var tabRegionAdapter = Scope.From(new TabRegionAdapter(viewFactory, shell.TabRegion));
			tabRegion.Adapt(tabRegionAdapter);
			shellBuilder.RegisterInstance<INavigate>(tabRegion);

			shellBuilder.Update(Container);

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