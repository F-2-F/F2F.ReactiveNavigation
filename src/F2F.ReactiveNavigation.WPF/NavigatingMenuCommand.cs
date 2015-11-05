using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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

		protected override Task Initialize()
		{
			base.Initialize();

			this.Command = ReactiveCommand.CreateAsyncTask(CanExecuteObservable, _ => _navigator.RequestNavigate<TViewModel>(ProvideNavigationParameters()));

			this.WhenNavigatedTo()
				.Where(p => p.IsUserNavigation())
				.Where(_ => IsEnabled)
				.Do(_ => this.Command.Execute(null))
				.Subscribe();

			return Task.FromResult(false);
		}

		protected virtual INavigationParameters ProvideNavigationParameters()
		{
			return NavigationParameters.UserNavigation;
		}

		protected virtual IObservable<bool> CanExecuteObservable
		{
			get { return Observable.Return(true); }
		}
	}
}
