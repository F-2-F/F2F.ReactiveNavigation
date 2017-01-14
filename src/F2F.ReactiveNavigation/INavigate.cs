using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation
{
    public interface INavigate
    {
        Task RequestNavigate<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel;

        Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters);

        Task RequestClose<TViewModel>(INavigationParameters parameters)
            where TViewModel : ReactiveViewModel;

        Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters);
    }

    public interface INavigate<T> : INavigate
    {
    }
}