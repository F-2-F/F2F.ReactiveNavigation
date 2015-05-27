using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.WPF
{
	public class NavigatingMenuCommand<TViewModel> : MenuCommand
		where TViewModel : ReactiveViewModel
	{
		private readonly INavigate _navigator;

		protected NavigatingMenuCommand(INavigate navigator)
		{
			if (navigator == null)
				throw new ArgumentNullException("navigator", "navigator is null.");
			
			_navigator = navigator;
		}

		protected override void Initialize()
		{
			base.Initialize();

			this.Command = ReactiveCommand.CreateAsyncTask(_ => _navigator.RequestNavigate<TViewModel>(ProvideNavigationParameters()));
		}

		protected virtual INavigationParameters ProvideNavigationParameters()
		{
			return NavigationParameters.Empty;
		}
	}
}
