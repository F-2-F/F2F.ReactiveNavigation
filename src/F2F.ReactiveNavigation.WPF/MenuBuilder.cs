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

		/// <summary>
		/// Constructor, sets the menu regionTarget
		/// </summary>
		/// <param name="regionTarget"></param>
		public MenuBuilder(Menu regionTarget)
		{
			if (regionTarget == null)
				throw new ArgumentNullException("regionTarget", "regionTarget is null.");

			_regionTarget = regionTarget;
		}

		/// <summary>
		/// Adds a MenuCommand into the Menu
		/// </summary>
		/// <param name="command">MenuCommand</param>
		public void AddMenuItem(IMenuCommand command)
		{
			if (command == null)
				throw new ArgumentNullException("command", "command is null.");

			var menuItem = new MenuCommandBridge(command);

			_regionTarget.Items.Add(menuItem);
		}

		/// <summary>
		/// Adds a new MenuCommand with and sets the action
		/// </summary>
		/// <param name="header">Menu item name</param>
		/// <param name="action"></param>
		public void AddMenuItem(string header, Action action)
		{
			if (String.IsNullOrEmpty(header))
				throw new ArgumentException("header is null or empty.", "header");
			if (action == null)
				throw new ArgumentNullException("action", "action is null.");

			var menuItem = new MenuCommandBridge(header, action);

			_regionTarget.Items.Add(menuItem);
		}

		/// <summary>
		/// Adds a new Menu item with several submenu commands
		/// </summary>
		/// <param name="header">Menu item name</param>
		/// <param name="submenuCommands">submenu commands</param>
		public void AddMenuItems(string header, IEnumerable<IMenuCommand> submenuCommands)
		{
			if (String.IsNullOrEmpty(header))
				throw new ArgumentException("header is null or empty.", "header");
			if (submenuCommands == null)
				throw new ArgumentNullException("submenuCommands", "submenuCommands is null.");

			var parentMenuCommand = new MenuCommandBridge(header);

			foreach (var command in submenuCommands)
			{
				parentMenuCommand.Items.Add(new MenuCommandBridge(command));
			}

			_regionTarget.Items.Add(parentMenuCommand);
		}

		/// <summary>
		/// Adds several submenu commands into a given parent menu command
		/// </summary>
		/// <param name="parent">Parent menu command</param>
		/// <param name="submenuCommands">Submenu commands</param>
		public void AddMenuItems(IMenuCommand parent, IEnumerable<IMenuCommand> submenuCommands)
		{
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null.");
			if (submenuCommands == null)
				throw new ArgumentNullException("submenuCommands", "commands is null.");

			var parentMenuCommand = GetParentMenuCommandBridge(parent);

			if(parentMenuCommand != null)
			{
				foreach (var submenuCommand in submenuCommands)
				{
					parentMenuCommand.Items.Add(new MenuCommandBridge(submenuCommand));
				}
			}
		}

		/// <summary>
		/// Adds a single submenu command into a given parent menu command
		/// </summary>
		/// <param name="parent">Parent menu command</param>
		/// <param name="submenuCommand">Submenu command</param>
		public void AddMenuItems(IMenuCommand parent, IMenuCommand submenuCommand)
		{
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null.");
			if (submenuCommand == null)
				throw new ArgumentNullException("submenuCommand", "command is null.");

			var parentMenuItem = GetParentMenuCommandBridge(parent);

			if (parentMenuItem != null)
			{
				parentMenuItem.Items.Add(new MenuCommandBridge(submenuCommand));
			}
		}

		private MenuCommandBridge GetParentMenuCommandBridge(IMenuCommand parentMenuCommand)
		{

			MenuCommandBridge parentMenuCommandBridge = null;

			foreach(MenuCommandBridge item in  _regionTarget.Items)
			{
				if (item.MenuCommand == parentMenuCommand)
				{
					parentMenuCommandBridge = item;
					break;
				}
			}

			return parentMenuCommandBridge;
		}
	}
}