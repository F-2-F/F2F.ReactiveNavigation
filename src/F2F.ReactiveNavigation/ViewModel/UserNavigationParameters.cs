using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	/// <summary>
	/// Navigation parameters that are passed through when navigation was initiated by
	///
	/// </summary>
	internal class UserNavigationParameters : INavigationParameters
	{
		private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();

		public UserNavigationParameters()
		{
		}

		public T Get<T>(string parameterName)
		{
			// TODO: Make robust, but effectively not used
			return (T)_parameters[parameterName];
		}

		public void Set<T>(string parameterName, T value)
		{
			// TODO: Make robust, but effectively not used
			_parameters[parameterName] = value;
		}
	}
}