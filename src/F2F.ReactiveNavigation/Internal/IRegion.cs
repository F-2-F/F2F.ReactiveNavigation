using F2F.ReactiveNavigation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using dbc = System.Diagnostics.Contracts;

namespace F2F.ReactiveNavigation.Internal
{
	[dbc.ContractClass(typeof(IRegionContract))]
	internal interface IRegion : ReactiveNavigation.IRegion
	{
		void Add(ReactiveViewModel viewModel);

		void Remove(ReactiveViewModel viewModel);

		void Activate(ReactiveViewModel viewModel);

		IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate);
	}

#pragma warning disable 0067  // suppress warning CS0067 "unused event" in contract classes

	[dbc.ContractClassFor(typeof(IRegion))]
	internal abstract class IRegionContract : IRegion
	{
		public string Name
		{
			get { return default(string); }
		}

		public IObservable<ReactiveViewModel> Added
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public IObservable<ReactiveViewModel> Removed
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public IObservable<ReactiveViewModel> Activated
		{
			get { return default(IObservable<ReactiveViewModel>); }
		}

		public void Add(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");
		}

		public void Remove(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");
			dbc.Contract.Requires<ArgumentException>(Contains(viewModel), "View model cannot be removed, since it is not contained in this region");
		}

		public void Activate(ReactiveViewModel viewModel)
		{
			dbc.Contract.Requires<ArgumentNullException>(viewModel != null, "viewModel is null");
			dbc.Contract.Requires<ArgumentException>(Contains(viewModel), "View model cannot be activated, since it is not contained in this region");
		}

		public bool Contains(ReactiveViewModel viewModel)
		{
			return default(bool);
		}

		public IEnumerable<ReactiveViewModel> Find(Func<ReactiveViewModel, bool> predicate)
		{
			dbc.Contract.Requires<ArgumentNullException>(predicate != null, "predicate is null");

			return default(IEnumerable<ReactiveViewModel>);
		}

		public Task RequestNavigate(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return default(Task);
		}

		public Task RequestClose(ReactiveViewModel navigationTarget, INavigationParameters parameters)
		{
			return default(Task);
		}
	}

#pragma warning restore
}