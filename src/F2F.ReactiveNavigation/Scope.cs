using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace System
{
	public static class Scope
	{
		

		private class ScopedLifetimeBuilder<T> : IScopedLifetimeBuilder<T>
			where T : class
		{
			private readonly T _object;

			public ScopedLifetimeBuilder(T @object)
			{
				if (@object == null)
					throw new ArgumentNullException("@object", "@object is null.");

				_object = @object;
			}

			public ScopedLifetime<T> EndingWith(IDisposable disposable)
			{
				if (disposable == null)
					throw new ArgumentNullException("disposable", "disposable is null.");

				return new ScopedLifetime<T>(_object, disposable);
			}

			public ScopedLifetime<T> EndingWith(Action disposeAction)
			{
				if (disposeAction == null)
					throw new ArgumentNullException("disposeAction", "disposeAction is null.");

				return new ScopedLifetime<T>(_object, Disposable.Create(disposeAction));
			}
		}

		public static IScopedLifetime<T> From<T>(T @object)
			where T : class, IDisposable
		{
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");

			return new ScopedLifetime<T>(@object, @object);
		}

		public static IScopedLifetime<T> From<T>(T @object, params IDisposable[] scope)
			where T : class
		{
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");
			if (scope == null)
				throw new ArgumentNullException("scope", "scope is null.");

			return new ScopedLifetime<T>(@object, new CompositeDisposable(scope));
		}

		public static IScopedLifetimeBuilder<T> For<T>(T @object)
			where T : class
		{
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");

			return new ScopedLifetimeBuilder<T>(@object);
		}

		public static IScopedLifetimeBuilder<T> Lifetime<T>(this T @object)
			where T : class
		{
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");

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
			if (@object == null)
				throw new ArgumentNullException("@object", "@object is null.");

			return new ScopedLifetimeBuilder<T>(@object).EndingWith(() => { });
		}
	}
}