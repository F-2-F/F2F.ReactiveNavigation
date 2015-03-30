using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.ReactiveNavigation;
using F2F.ReactiveNavigation.WPF;
using F2F.WPF.Sample.View;
using F2F.WPF.Sample.ViewModel;
using Microsoft.Practices.Unity;
using Autofac;

namespace F2F.WPF.Sample
{
	internal class SampleInitializer : IInitializer
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