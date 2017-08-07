using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.WPF.Sample.Controller
{
    public class SampleController : ISampleController
    {
        public string LoadTitle(int value)
        {
            return value.ToString();
        }
    }
}