using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.ViewModel
{
	public class UserNavigationParameters : INavigationParameters
	{
		public T Get<T>(string parameterName)
		{
			return default(T);
		}

		public bool Has(string parameterName)
		{
			return false;
		}
	}
}