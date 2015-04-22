using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace F2F.ReactiveNavigation.ViewModel
{
	internal class NavigationObservable<T> : INavigationObservable<T>
	{
		private class IndicateException<U>
		{
			public bool IsFaulted { get; set; }

			public U Object { get; set; }
		}

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

		public IObservable<T> ToObservable()
		{
			return _observable;
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
					.Select(p => new IndicateException<T>() { Object = p })
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.Do(p => syncAction(p.Object))
					.Catch<IndicateException<T>, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(new IndicateException<T>() { IsFaulted = true });
					})
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(false))
					.Where(p => !p.IsFaulted)
					.Select(p => p.Object));
		}

		public INavigationObservable<TResult> Do<TResult>(Func<T, TResult> syncAction)
		{
			return new NavigationObservable<TResult>(_viewModel,
				_observable
					.ObserveOn(RxApp.MainThreadScheduler)
					.Select(p => new IndicateException<T>() { Object = p })
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.Select(p => syncAction(p.Object))
					.Select(p => new IndicateException<TResult>() { Object = p })
					.Catch<IndicateException<TResult>, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(new IndicateException<TResult>() { IsFaulted = true });
					})
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(false))
					.Where(p => !p.IsFaulted)
					.Select(p => p.Object));
		}

		public INavigationObservable<T> DoAsync(Func<T, Task> asyncAction)
		{
			return new NavigationObservable<T>(_viewModel,
				_observable
					.ObserveOn(RxApp.TaskpoolScheduler)
					.Select(p => new IndicateException<T>() { Object = p })
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.SelectMany(async p =>
					{
						await asyncAction(p.Object);

						_viewModel.AsyncNavigatingSource.OnNext(false);

						return p;
					})
					.Catch<IndicateException<T>, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(new IndicateException<T>() { IsFaulted = true });
					})
					.Where(p => !p.IsFaulted)
					.Select(p => p.Object));
		}

		public INavigationObservable<TResult> DoAsync<TResult>(Func<T, Task<TResult>> asyncAction)
		{
			return new NavigationObservable<TResult>(_viewModel,
				_observable
					.ObserveOn(RxApp.TaskpoolScheduler)
					.Select(p => new IndicateException<T>() { Object = p })
					.Do(_ => _viewModel.AsyncNavigatingSource.OnNext(true))
					.SelectMany(async p =>
					{
						var r = await asyncAction(p.Object);

						_viewModel.AsyncNavigatingSource.OnNext(false);

						return r;
					})
					.Select(p => new IndicateException<TResult>() { Object = p })
					.Catch<IndicateException<TResult>, Exception>(ex =>
					{
						_viewModel.ThrownExceptionsSource.OnNext(ex);
						return Observable.Return(new IndicateException<TResult>() { IsFaulted = true });
					})
					.Where(p => !p.IsFaulted)
					.Select(p => p.Object));
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