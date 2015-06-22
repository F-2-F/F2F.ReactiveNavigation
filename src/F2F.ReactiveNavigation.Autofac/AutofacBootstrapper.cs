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
		private IContainer _container = new ContainerBuilder().Build();

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
				.RegisterType<RegionContainer>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.Update(Container);
		}

		public void Dispose()
		{
			if (_container != null)
			{
				_container.Dispose();
				_container = null;
			}
		}

		//private void RegisterInitializers()
		//{
		//	var builder = new ContainerBuilder();

		//	Initializers
		//		.Select(t => builder.RegisterType(t).As(t))
		//		.ToList();

		//	builder.Update(Container);
		//}

		//protected void RunInitializers()
		//{
		//	RegisterInitializers();

		//	var builder = new ContainerBuilder();

		//	Initializers
		//		.Select(t => Container.Resolve(t))
		//		.Cast<IInitializer>()
		//		.ToList()
		//		.ForEach(i => i.Initialize(builder));

		//	builder.Update(Container);
		//}

		//protected abstract IEnumerable<Type> Initializers { get; }

		//protected abstract void RegisterViewFactory(ContainerBuilder builder);
	}
}