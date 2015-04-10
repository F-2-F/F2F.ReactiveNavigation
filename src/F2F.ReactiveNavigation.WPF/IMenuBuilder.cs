using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.WPF
{
	public interface IMenuBuilder
	{
		void AddMenuItem(IMenuCommand command);

		void AddMenuItem(string header, Action action);
	}
}