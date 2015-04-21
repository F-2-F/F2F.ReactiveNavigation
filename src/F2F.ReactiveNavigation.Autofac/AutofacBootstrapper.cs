﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;

namespace F2F.ReactiveNavigation.Autofac
{
	public abstract class AutofacBootstrapper : IBootstrapper
	{
		private readonly IContainer _container = new ContainerBuilder().Build();

		public IContainer Container
		{
			get { return _container; }
		}

		public virtual void Run()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<RegionContainer>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder
				.RegisterType<ViewFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder
				.RegisterType<AutofacViewModelFactory>()
				.AsImplementedInterfaces();

			builder
				.Register(c => new RegionContainer(c.Resolve<ICreateViewModel>(), RxApp.MainThreadScheduler))
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.Update(Container);
		}

		public void RegisterBootstrapper(Assembly asm)
		{
			var builder = new ContainerBuilder();

			builder
				.RegisterAssemblyTypes(asm)
				.Where(t => typeof(IBootstrapper).IsAssignableFrom(t))
				.As<IBootstrapper>();

			builder.Update(Container);
		}

		public void RunRegisteredBootstrappers()
		{
			var initializers = Container.Resolve<IEnumerable<IBootstrapper>>();
			initializers.ToList().ForEach(i => i.Run());
		}

		public void RegisterViewModels(Assembly asm)
		{
			var builder = new ContainerBuilder();

			builder
				.RegisterAssemblyTypes(asm)
				.Where(t => typeof(ReactiveViewModel).IsAssignableFrom(t))
				.Keyed<ReactiveViewModel>(t => t);

			builder.Update(Container);
		}
	}
}