using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.WPF
{
    public class NavigatingMenuCommand<TViewModel> : MenuCommand
        where TViewModel : ReactiveViewModel
    {
        private readonly INavigate _navigator;

        protected NavigatingMenuCommand(INavigate navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException("navigator", "navigator is null.");
            
            _navigator = navigator;
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            this.Command = ReactiveCommand.CreateFromTask<object>(_ => _navigator.RequestNavigate<TViewModel>(ProvideNavigationParameters()), CanExecuteObservable);

            this.WhenNavigatedTo()
                .Where(p => p.IsUserNavigation())
                .Where(_ => IsEnabled)
                .Do(_ => this.Command.Execute(null))
                .Subscribe();
        }

        protected virtual INavigationParameters ProvideNavigationParameters()
        {
            return NavigationParameters.UserNavigation;
        }

        protected virtual IObservable<bool> CanExecuteObservable
        {
            get { return Observable.Return(true); }
        }
    }
}
