using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace F2F.ReactiveNavigation.WPF
{
	public interface IMenuCommand : IHaveTitle
	{
		ICommand Command { get; }
	}
}