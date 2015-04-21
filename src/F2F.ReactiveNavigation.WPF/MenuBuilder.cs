using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF
{
	public class MenuBuilder : IMenuBuilder
	{
		private readonly Menu _regionTarget;

		public MenuBuilder(Menu regionTarget)
		{
			if (regionTarget == null)
				throw new ArgumentNullException("regionTarget", "regionTarget is null.");

			_regionTarget = regionTarget;
		}

		public void AddMenuItem(IMenuCommand command)
		{
			if (command == null)
				throw new ArgumentNullException("command", "command is null.");

			var menuItem = new MenuItem()
			{
				Header = command.Title,
				Command = command.Command
			};

			_regionTarget.Items.Add(menuItem);
		}

		public void AddMenuItem(string header, Action action)
		{
			if (String.IsNullOrEmpty(header))
				throw new ArgumentException("header is null or empty.", "header");
			if (action == null)
				throw new ArgumentNullException("action", "action is null.");

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