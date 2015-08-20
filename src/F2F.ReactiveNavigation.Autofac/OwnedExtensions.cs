using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac.Features.OwnedInstances
{
	public static class OwnedExtensions
	{
		public static ScopedLifetime<T> ToScopedLifetime<T>(this Owned<T> owned)
			where T : class
		{
			return owned.Value.Lifetime().EndingWith(owned);
		}
	}
}
