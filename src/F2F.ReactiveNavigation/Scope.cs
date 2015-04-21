using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using F2F.ReactiveNavigation;
using dbc = System.Diagnostics.Contracts;

namespace System
{
	public static class Scope
	{
		public interface IScopedLifetimeBuilder<T>
			where T : class
		{
			/// <summary>
			/// Lifetime of object can be ended with disposing the given disposable.
			/// </summary>
			/// <param name="disposable"></param>
			/// <returns></returns>
			ScopedLifetime<T> EndingWith(IDisposable disposable);

			/// <summary>
			/// Lifetime of object can be ended with executing the given disposeAction.
			/// </summary>
			/// <param name="disposeAction"></param>
			/// <returns></returns>
			ScopedLifetime<T> EndingWith(Action disposeAction);
		}

		private class ScopedLifetimeBuilder<T> : IScopedLifetimeBuilder<T>
			where T : class
		{
			private readonly T _object;

			public ScopedLifetimeBuilder(T @object)
			{
				dbc.Contract.Requires<ArgumentNullException>(@object != null, "object must not be null");

				_object = @object;
			}

			public ScopedLifetime<T> EndingWith(IDisposable disposable)
			{
				return new ScopedLifetime<T>(_object, disposable);
			}

			public ScopedLifetime<T> EndingWith(Action disposeAction)
			{
				return new ScopedLifetime<T>(_object, Disposable.Create(disposeAction));
			}
		}

		public static IScopedLifetime<T> From<T>(T @object)
			where T : class, IDisposable
		{
			return new ScopedLifetime<T>(@object, @object);
		}

		public static IScopedLifetime<T> From<T>(T @object, params IDisposable[] scope)
			where T : class
		{
			return new ScopedLifetime<T>(@object, new CompositeDisposable(scope));
		}

		public static IScopedLifetimeBuilder<T> For<T>(T @object)
			where T : class
		{
			return new ScopedLifetimeBuilder<T>(@object);
		}

		public static IScopedLifetimeBuilder<T> Lifetime<T>(this T @object)
			where T : class
		{
			return new ScopedLifetimeBuilder<T>(@object);
		}

		/// <summary>
		/// Returns a scoped lifetime object with an empty disposable. This effectively
		/// represents an unscoped lifetime for the given object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="object"></param>
		/// <returns></returns>
		public static IScopedLifetime<T> WithUnscopedLifetime<T>(this T @object)
			where T : class
		{
			return new ScopedLifetimeBuilder<T>(@object).EndingWith(() => { });
		}
	}
}