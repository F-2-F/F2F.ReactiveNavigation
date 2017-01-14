using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace F2F.ReactiveNavigation.Autofac
{
    public interface IInitializer
    {
        void Initialize(ContainerBuilder builder);
    }
}