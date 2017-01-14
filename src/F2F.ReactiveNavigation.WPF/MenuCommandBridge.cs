using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ReactiveUI;

namespace F2F.ReactiveNavigation.WPF
{
    public class MenuCommandBridge : MenuItem
    {
        private IMenuCommand _menuCommand;

        public MenuCommandBridge(IMenuCommand menuCommand)
        {
            if (menuCommand == null)
                throw new ArgumentNullException("menuCommand", "menuCommand is null.");

            _menuCommand = menuCommand;
            this.Header = menuCommand.Title;
            this.Command = menuCommand.Command;
        }

        public MenuCommandBridge(string header, Action action)
        {
            if (String.IsNullOrEmpty(header))
                throw new ArgumentException("header is null or empty.", "header");
            if (action == null)
                throw new ArgumentNullException("action", "action is null.");

            this.Header = header;

            var command = ReactiveCommand.Create();
            command.Subscribe(_ => action());
            this.Command = command;
            
            _menuCommand = new MenuCommand() { Title = header };
        }

        public MenuCommandBridge(string header)
        {
            if (String.IsNullOrEmpty(header))
                throw new ArgumentException("header is null or empty.", "header");

            this.Header = header;

            _menuCommand = new MenuCommand() { Title = header };
        }

        public IMenuCommand MenuCommand
        {
            get { return _menuCommand; }
        }
    }
}
