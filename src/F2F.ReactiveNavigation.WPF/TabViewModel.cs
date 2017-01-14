using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF
{
    internal class TabViewModel
    {
        private readonly INavigate _router;
        private readonly ReactiveViewModel _childViewModel;

        public TabViewModel(INavigate router, ReactiveViewModel childViewModel)
        {
            if (router == null)
                throw new ArgumentNullException("router", "router is null.");
            if (childViewModel == null)
                throw new ArgumentNullException("childViewModel", "childViewModel is null.");

            _router = router;
            _childViewModel = childViewModel;

            Close = ReactiveCommand.Create();
            Close.Subscribe(_ => Router.RequestClose(ChildViewModel, NavigationParameters.UserNavigation));
        }

        public INavigate Router
        {
            get { return _router; }
        }

        public ReactiveViewModel ChildViewModel
        {
            get { return _childViewModel; }
        }

        public ReactiveCommand<object> Close { get; protected set; }
    }
}