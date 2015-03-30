using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public static class NavigationParameters
	{
		private class EmptyNavigationParameters : INavigationParameters
		{
			public T Get<T>(string parameterName)
			{
				return default(T);
			}

			public void Set<T>(string parameterName, T value)
			{
			}
		}

		private class DictionaryNavigationParameters : INavigationParameters
		{
			private readonly IDictionary<string, object> _parameters;

			public DictionaryNavigationParameters(IDictionary<string, object> parameters)
			{
				_parameters = parameters;
			}

			public T Get<T>(string parameterName)
			{
				return (T)_parameters[parameterName];
			}

			public void Set<T>(string parameterName, T value)
			{
				_parameters[parameterName] = value;
			}
		}

		private static INavigationParameters _empty;

		public static INavigationParameters Empty
		{
			get
			{
				if (_empty == null)
				{
					_empty = new EmptyNavigationParameters();
				}
				return _empty;
			}
		}

		public static INavigationParameters Create(IDictionary<string, object> parameters)
		{
			return new DictionaryNavigationParameters(parameters);
		}

		public static INavigationParameters Create()
		{
			return new DictionaryNavigationParameters(new Dictionary<string, object>());
		}

		public static INavigationParameters UserNavigation()
		{
			return new UserNavigationParameters();
		}

		public static bool IsUserNavigation(this INavigationParameters parameters)
		{
			return parameters is UserNavigationParameters;
		}
	}
}