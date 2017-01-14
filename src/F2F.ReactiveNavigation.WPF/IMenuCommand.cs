using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;

namespace F2F.ReactiveNavigation.WPF
{
    public interface IMenuCommand : IHaveTitle, ISortable, INotifyPropertyChanged
    {
        ReactiveCommand<Unit> Command { get; }

        bool IsEnabled { get; }
    }
}