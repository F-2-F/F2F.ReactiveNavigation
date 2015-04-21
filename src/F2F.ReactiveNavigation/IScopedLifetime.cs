using System.Collections.Generic;
using System.Linq;

namespace System
{
	public interface IScopedLifetime<out T> : IDisposable
			where T : class
	{
		T Object { get; }

		IDisposable Scope { get; }
	}
}