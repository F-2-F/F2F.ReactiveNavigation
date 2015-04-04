using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.WPF
{
	public class MenuBuilder : IMenuBuilder
	{
		private readonly Menu _regionTarget;

		public MenuBuilder(Menu regionTarget)
		{
			dbc.Contract.Requires<ArgumentNullException>(regionTarget != null, "regionTarget must not be null");

			_regionTarget = regionTarget;
		}

		public void AddMenuItem(IMenuCommand command)
		{
			var menuItem = new MenuItem()
			{
				Header = command.Title,
				Command = command.Command
			};

			_regionTarget.Items.Add(menuItem);
		}

		public void AddMenuItem(string header, Action action)
		{
			var command = ReactiveCommand.Create();
			command.Subscribe(_ => action());
			
			var menuItem = new MenuItem()
			{
				Header = header,
				Command = command
			};

			_regionTarget.Items.Add(menuItem);
		}
	}
}