using System;
using System.Collections.Generic;
using System.Linq;
using F2F.ReactiveNavigation.ViewModel;
using ReactiveUI;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.WPF
{
	internal class TabViewModel : ReactiveViewModel
	{
		private readonly INavigate _router;
		private readonly ReactiveViewModel _childViewModel;

		public TabViewModel(INavigate router, ReactiveViewModel childViewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(router != null, "router must not be null");
			dbc.Contract.Requires<ArgumentNullException>(childViewModel != null, "childViewModel must not be null");

			_router = router;
			_childViewModel = childViewModel;
		}

		protected override void Initialize()
		{
			Close = ReactiveCommand.Create();
			Close.Subscribe(_ => Router.RequestClose(ChildViewModel, NavigationParameters.UserNavigation()));
		}

		public INavigate Router
		{
			get { return _router; }
		}

		public ReactiveViewModel ChildViewModel
		{
			get { return _childViewModel; }
		}

		public ReactiveCommand<object> Close { get; protected set; }
	}
}