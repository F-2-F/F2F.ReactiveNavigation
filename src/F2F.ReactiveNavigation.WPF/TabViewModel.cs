using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using dbc = System.Diagnostics.Contracts;
using ReactiveUI;
using System.Reactive;

namespace F2F.ReactiveNavigation.WPF
{
	public class TabViewModel : ReactiveViewModel
	{
		private readonly IRegion _region;
		private readonly ReactiveViewModel _childViewModel;

		public TabViewModel(IRegion region, ReactiveViewModel childViewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(region != null, "region must not be null");
			dbc.Contract.Requires<ArgumentNullException>(childViewModel != null, "childViewModel must not be null");

			_region = region;
			_childViewModel = childViewModel;
		}

		protected override void Initialize()
		{
			Close = ReactiveCommand.CreateAsyncTask(_ => Region.RequestClose(ChildViewModel, NavigationParameters.UserNavigation()));
		}

		public IRegion Region
		{
			get { return _region; }
		}

		public ReactiveViewModel ChildViewModel
		{
			get { return _childViewModel; }
		}

		public ReactiveCommand<Unit> Close { get; protected set; }
	}
}