using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF.Autofac;
using F2F.ReactiveNavigation.WPF.Sample.ViewModel;

namespace F2F.ReactiveNavigation.WPF.Sample
{
	internal class Bootstrapper : IDisposable
	{
		private static int _viewModelCount = 0;

		private IContainer _container;

		public void Run()
		{
			var builder = new ContainerBuilder();

			builder.RegisterReactiveNavigation();

			builder
				.RegisterType<AutofacViewFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterModule<SampleModule>();

			InitializeNavigationRegions(builder);

			ShowShell(builder);
		}

		private void InitializeNavigationRegions(ContainerBuilder builder)
		{
			builder.RegisterMultiItemsRegion<Regions.TabRegion>();
		}

		private void ShowShell(ContainerBuilder builder)
		{
			builder.RegisterType<MainWindow>()
				.SingleInstance()
				.OnActivated(e =>
				{
					var shell = e.Instance;

					var regionContainer = e.Context.Resolve<IRegionContainer>();
					var tabRegion = regionContainer.GetRegion<Regions.TabRegion>();
					var viewFactory = e.Context.Resolve<ICreateView>();
					var tabRegionAdapter = Scope.From(new TabRegionAdapter(viewFactory, shell.TabRegion));
					tabRegion.Adapt(tabRegionAdapter);

					var menuBuilder = new MenuBuilder(shell.MenuRegion);
					menuBuilder.AddMenuItem("Add", () => AddNewView(tabRegion));
					menuBuilder.AddMenuItems("GroupOfItems", GetMenuCommands());

					var parentMenuItem = CreateParentMenuCommand();
					menuBuilder.AddMenuItem(parentMenuItem);
					menuBuilder.AddMenuItems(parentMenuItem, GetMenuCommands());

					menuBuilder.AddMenuItems(parentMenuItem, new MenuCommand() { Title = "Added later" });

					menuBuilder.AddMenuItem("Other", () => tabRegion.RequestNavigate<OtherViewModel>(NavigationParameters.UserNavigation));
					menuBuilder.AddMenuItem("Close all", () => tabRegion.CloseAll());
				});

			_container = builder.Build();

			Application.Current.MainWindow = _container.Resolve<MainWindow>();
			Application.Current.MainWindow.Show();
		}

		private MenuCommand CreateParentMenuCommand()
		{
			return new MenuCommand() { Title = "I am your father" };
		}

		private static void AddNewView(IAdaptableRegion tabRegion)
		{
			var naviParams = NavigationParameters.Create()
				.Add("value", _viewModelCount++);
			tabRegion.RequestNavigate<SampleViewModel>(naviParams);
		}

		private IEnumerable<IMenuCommand> GetMenuCommands()
		{
			yield return new MenuCommand() { Title = "item1" };
			yield return new MenuCommand() { Title = "item2" };
			yield return new MenuCommand() { Title = "item3" };
		}

		public void Dispose()
		{
			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}
	}
}