using Autofac.Core;
using F2F.ReactiveNavigation.ViewModel;
using F2F.ReactiveNavigation.WPF;
using System;
using System.Linq;
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

        public static void RegisterView<TView, TViewModel>(this ContainerBuilder builder, Action<IActivatedEventArgs<TView>> onActivated)
            where TView : FrameworkElement
            where TViewModel : ReactiveViewModel
        {
            builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel)).OnActivated(onActivated);
        }

        public static void RegisterSingleInstanceView<TView, TViewModel>(this ContainerBuilder builder)
            where TView : FrameworkElement
            where TViewModel : ReactiveViewModel
        {
            builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel)).SingleInstance();
        }

        public static void RegisterSingleInstanceView<TView, TViewModel>(this ContainerBuilder builder, Action<IContainer> buildCallback)
            where TView : FrameworkElement
            where TViewModel : ReactiveViewModel
        {
            builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel)).SingleInstance();

            builder.RegisterBuildCallback(buildCallback);
        }

        public static void RegisterAutoActivatedSingleInstanceView<TView, TViewModel>(this ContainerBuilder builder, Action<IContainer> buildCallback)
            where TView : FrameworkElement
            where TViewModel : ReactiveViewModel
        {
            builder.RegisterType<TView>().Keyed<FrameworkElement>(typeof(TViewModel)).SingleInstance().AutoActivate();

            builder.RegisterBuildCallback(buildCallback);
        }
    }
}