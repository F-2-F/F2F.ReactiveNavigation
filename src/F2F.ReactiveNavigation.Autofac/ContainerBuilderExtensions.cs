using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.ViewModel;

namespace Autofac
{
	public static class ContainerBuilderExtensions
	{
		public static void RegisterViewModels(this ContainerBuilder builder, Assembly assembly)
		{
			builder
				.RegisterAssemblyTypes(assembly)
				.AssignableTo<ReactiveViewModel>()
				.AsSelf()
				.As<ReactiveViewModel>()
				.Keyed<ReactiveViewModel>(t => t);
		}

		public static void RegisterViewModel<TViewModel>(this ContainerBuilder builder)
			where TViewModel : ReactiveViewModel
		{
			builder
				.RegisterType<TViewModel>()
				.AsSelf()
				.As<ReactiveViewModel>()
				.Keyed<ReactiveViewModel>(typeof(TViewModel));
		}

		public static void RegisterSingleInstanceViewModel<TViewModel>(this ContainerBuilder builder)
			where TViewModel : ReactiveViewModel
		{
			builder
				.RegisterType<TViewModel>()
				.AsSelf()
				.As<ReactiveViewModel>()
				.SingleInstance()
				.Keyed<ReactiveViewModel>(typeof(TViewModel));
		}

		public static void RegisterMultiItemsRegion<TRegionType>(this ContainerBuilder builder)
		{
			builder
				.Register<INavigate<TRegionType>>(
					ctx => new Navigate<TRegionType>(
								ctx.Resolve<IRegionContainer>().CreateMultiItemsRegion(typeof(TRegionType).ToString())))
				.As<INavigate<TRegionType>>()
				.SingleInstance()
				.AutoActivate();
		}

		public static void RegisterSingleItemRegion<TRegionType>(this ContainerBuilder builder)
		{
			builder
				.Register<INavigate<TRegionType>>(
					ctx => new Navigate<TRegionType>(
								ctx.Resolve<IRegionContainer>().CreateSingleItemRegion(typeof(TRegionType).ToString())))
				.As<INavigate<TRegionType>>()
				.SingleInstance()
				.AutoActivate();
		}
	}
}
