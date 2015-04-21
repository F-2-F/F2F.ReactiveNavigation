using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using dbc = System.Diagnostics.Contracts;

namespace System
{
	/// <summary>
	/// Wraps an arbitrary reference type and a disposable. When the lifetime of the
	/// wrapped object should end, Dispose() must be called on this object. Instead
	/// of demanding the wrapped object to implement IDiposable, we demand an explicit
	/// IDisposable. This has several advantages over only the object being IDisposable.
	/// E.g. when composing the object with an IoC container, the IoC container can
	/// track all disposable instances that it used during composition of the object.
	/// So we don't have a cascade of IDisposable implementations to do, we rely on the IoC
	/// container to hand us a single IDisposable that can clean up the whole object tree
	/// at once.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ScopedLifetime<T> : IDisposable
		where T : class
	{
		private readonly T _object;
		private IDisposable _scope;

		public ScopedLifetime(T @object, IDisposable scope)
		{
			dbc.Contract.Requires<ArgumentNullException>(@object != null, "object must not be null");
			dbc.Contract.Requires<ArgumentNullException>(scope != null, "scope must not be null");

			_object = @object;
			_scope = scope;
		}

		public T Object
		{
			get { return _object; }
		}

		public IDisposable Scope
		{
			get { return _scope; }
		}

		public void Dispose()
		{
			if (_scope != null)
			{
				_scope.Dispose();
				_scope = null;
			}
		}
	}
}