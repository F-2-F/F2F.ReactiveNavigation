using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	internal class NavigationObservable<T> : INavigationObservable<T>
		where T : class
	{
		private readonly ReactiveViewModel _viewModel;
		private readonly IObservable<T> _observable;

		public NavigationObservable(ReactiveViewModel viewModel, IObservable<T> observable)
		{
			_viewModel = viewModel;
			_observable = observable;
		}

		public ReactiveViewModel ViewModel
		{
			get { return _viewModel; }
		}

		public INavigationObservable<T> Where(Func<T, bool> predicate)
		{
			return new NavigationObservable<T>(_viewModel, _observable.Where(predicate));
		}

		public INavigationObservable<T> Do(Action<T> syncAction)
		{
			return new NavigationObservable<T>(_viewModel,
				_observable
					.ObserveOn(RxApp.MainThreadScheduler)
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.Do(syncAction)
					.Catch<T, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return<T>(default(T));
					})
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(false))
					.Where(p => p != default(T)));
		}

		public INavigationObservable<T> DoAsync(Func<T, Task> asyncAction)
		{
			return new NavigationObservable<T>(_viewModel,
				_observable
					.ObserveOn(RxApp.TaskpoolScheduler)
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.SelectMany(async p =>
					{
						await asyncAction(p);
						return p;
					})
					.Catch<T, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return<T>(default(T));
					})
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(false))
					.Where(p => p != default(T)));
		}

		private static IObservable<T> StartBusy(ReactiveViewModel viewModel, IObservable<T> observable)
		{
			return observable.Do(_ => viewModel.AsyncNavigatingSource.OnNext(true));
		}

		private static IObservable<T> StopBusy(ReactiveViewModel viewModel, IObservable<T> observable)
		{
			return observable
				.Catch<T, Exception>(ex =>
				{
					viewModel.ThrownExceptionsSource.OnNext(ex);
					return Observable.Return<T>(default(T));
				})
				.Do(_ => viewModel.AsyncNavigatingSource.OnNext(false));
		}

		public IDisposable Subscribe()
		{
			return _observable.Subscribe();
		}
	}
}