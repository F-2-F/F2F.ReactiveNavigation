using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.WPF;
using F2F.ReactiveNavigation.WPF.Sample.View;
using F2F.ReactiveNavigation.WPF.Sample.ViewModel;

namespace F2F.ReactiveNavigation.WPF.Sample
{
	internal class SampleInitializer : IBootstrapper
	{
		private readonly IRegisterViewFactories _viewFactoryCatalog;

		public SampleInitializer(IRegisterViewFactories viewFactoryCatalog)
		{
			_viewFactoryCatalog = viewFactoryCatalog;
		}

		public void Initialize()
		{
			_viewFactoryCatalog.Register<SampleViewModel>(vm => new SampleView { DataContext = vm });
		}
	}
}