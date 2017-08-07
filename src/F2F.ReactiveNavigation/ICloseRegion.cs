using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
    public interface ICloseRegion
    {
        Task CloseAll();
    }
}