using System;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.WPF.Sample.ViewModel
{
    public class OtherViewModel : ReactiveViewModel
    {
        protected override async Task Initialize()
        {
            await base.Initialize();

            Task.Delay(1000).Wait();    // intentionally block to see busy indication during initialization
        }
    }
}