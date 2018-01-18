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
        private IContainer _container = null;

        public IContainer Container
        {
            get
            {
                if (_container == null)
                    throw new NotSupportedException("You need to execute RunAsync() before Container is available.");

                return _container;
            }
        }

        public async Task RunAsync()
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

            await BootstrapAsync(builder);

            _container = builder.Build();
        }

        protected abstract Task BootstrapAsync(ContainerBuilder builder);

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }
    }
}