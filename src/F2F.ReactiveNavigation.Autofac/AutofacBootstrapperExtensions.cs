using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.Autofac
{
	public static class AutofacBootstrapperExtensions
	{
		public static void RegisterViewModels(this AutofacBootstrapper self, Assembly asm)
		{
			var builder = new ContainerBuilder();

			builder
				.RegisterAssemblyTypes(asm)
				.Where(t => typeof(ReactiveViewModel).IsAssignableFrom(t))
				.Keyed<ReactiveViewModel>(t => t);

			builder.Update(self.Container);
		}
	}
}
