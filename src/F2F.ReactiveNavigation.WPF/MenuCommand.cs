using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace F2F.ReactiveNavigation.WPF
{
	public class MenuCommand : IMenuCommand
	{
		public string Title { get; set; }

		public ICommand Command { get; set; }
	}
}