using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Autofac
{
	public static class ContainerBuilderExtensions
	{
		public static void RegisterMenuCommand<TMenuCommand>(this ContainerBuilder builder, Func<IComponentContext, TMenuCommand> resolveCommand)
			where TMenuCommand : MenuCommand
		{
			builder.Register<ReactiveViewModel>(resolveCommand).Keyed<ReactiveViewModel>(typeof(TMenuCommand));
		}

		public static void RegisterView<TView, TViewModel>(this ContainerBuilder builder)
			where TView : FrameworkElement
			where TViewModel : ReactiveViewModel
		{
			builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel));
		}

		public static void RegisterSingleInstanceView<TView, TViewModel>(this ContainerBuilder builder)
			where TView : FrameworkElement
			where TViewModel : ReactiveViewModel
		{
			builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel)).SingleInstance();
		}
	}
}
