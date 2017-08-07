using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using F2F.ReactiveNavigation.ViewModel;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.WPF
{
    public class MenuCommand : ReactiveViewModel, IMenuCommand
    {
        private bool _isEnabled;
        private int _sortHint;
        private ReactiveCommand<object, Unit> _command;

        public MenuCommand()
        {
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            this.WhenAnyValue(x => x.Command)
                .Where(c => c != null)
                .Do(c =>
                    c.CanExecute
                        .Do(e => IsEnabled = e)
                        .Subscribe()) // TODO: shall we track the subscription?
                .Subscribe();
        }

        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            private set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        public int SortHint
        {
            get { return _sortHint; }
            set { this.RaiseAndSetIfChanged(ref _sortHint, value); }
        }

        public ReactiveCommand<object, Unit> Command
        {
            get { return _command; }
            set { this.RaiseAndSetIfChanged(ref _command, value); }
        }
    }
}