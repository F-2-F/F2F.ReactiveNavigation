using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using F2F.ReactiveNavigation.ViewModel;

namespace F2F.ReactiveNavigation.WPF
{
	public class MenuCommand : ReactiveViewModel, IMenuCommand
	{
		private int _sortHint;
		private ReactiveCommand<Unit> _command;

		public MenuCommand()
		{
		}

		public bool IsEnabled
		{
			get { return Command != null ? Command.CanExecute(null) : true; }
		}

		public int SortHint
		{
			get { return _sortHint; }
			set { this.RaiseAndSetIfChanged(ref _sortHint, value); }
		}

		public ReactiveCommand<Unit> Command
		{
			get { return _command; }
			set { this.RaiseAndSetIfChanged(ref _command, value); }
		}
	}
}