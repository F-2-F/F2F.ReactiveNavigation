using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using F2F.ReactiveNavigation.WPF.Sample.Controller;
using F2F.ReactiveNavigation.WPF.Sample.View;
using F2F.ReactiveNavigation.WPF.Sample.ViewModel;

using a = Autofac;

namespace F2F.ReactiveNavigation.WPF.Sample
{
	internal class SampleModule : a.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder
				.RegisterType<SampleController>()
				.AsImplementedInterfaces();

			builder.RegisterViewModels(GetType().Assembly);

			builder.RegisterView<SampleView, SampleViewModel>();
			builder.RegisterView<SampleView, OtherViewModel>();
		}
	}
}