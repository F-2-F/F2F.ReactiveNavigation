using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
    public class Navigate<T> : INavigate<T>
    {
        private readonly INavigate _navigate;

        public Navigate(INavigate navigate)
        {
            _navigate = navigate;
        }

        public Task RequestNavigate<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            return _navigate.RequestNavigate<TViewModel>(parameters);
        }

        public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            return _navigate.RequestNavigate(navigationTarget, parameters);
        }

        public Task RequestClose<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel
        {
            return _navigate.RequestClose<TViewModel>(parameters);
        }

        public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
        {
            return _navigate.RequestClose(navigationTarget, parameters);
        }
    }
}
