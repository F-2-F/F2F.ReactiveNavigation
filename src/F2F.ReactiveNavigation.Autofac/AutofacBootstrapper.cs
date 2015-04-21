using System;
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

			builder
				.RegisterType<ViewFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder
				.RegisterType<AutofacViewModelFactory>()
				.AsImplementedInterfaces();

			builder
				.Register(c => new RegionContainer(c.Resolve<ICreateViewModel>(), RxApp.MainThreadScheduler))
				.As<IRegionContainer>()
				.SingleInstance();

			builder.Update(Container);
		}

		protected void RegisterInitializers(Assembly asm)
		{
			var builder = new ContainerBuilder();

			builder
				.RegisterAssemblyTypes(asm)
				.Where(t => typeof(IInitializer).IsAssignableFrom(t))
				.As<IInitializer>();

			builder.Update(Container);
		}

		protected void RunInitializers()
		{
			var initializers = Container.Resolve<IEnumerable<IInitializer>>();
			initializers.ToList().ForEach(i => i.Initialize());
		}

		protected void RegisterViewModels(Assembly asm)
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