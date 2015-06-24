using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

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

			this.Command = ReactiveCommand.CreateAsyncTask(CanExecuteObservable, _ => _navigator.RequestNavigate<TViewModel>(ProvideNavigationParameters()));

			this.WhenNavigatedTo()
				.Where(p => p.IsUserNavigation())
				.Where(_ => IsEnabled)
				.Do(_ => this.Command.Execute(null))
				.Subscribe();
		}

		protected virtual INavigationParameters ProvideNavigationParameters()
		{
			return NavigationParameters.Empty;
		}

		protected virtual IObservable<bool> CanExecuteObservable
		{
			get { return Observable.Return(true); }
		}
	}
}
