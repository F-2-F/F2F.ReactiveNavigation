using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.WPF.Sample.ViewModel
{
	public class OtherViewModel : ReactiveViewModel
	{
		protected override void Initialize()
		{
			Task.Delay(1000).Wait();
		}
	}
}