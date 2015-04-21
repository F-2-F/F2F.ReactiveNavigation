using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public static class NavigationParameters
	{
		private class DictionaryNavigationParameters : INavigationParameterSetter
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

			public INavigationParameterSetter Add<T>(string parameterName, T parameterValue)
			{
				_parameters[parameterName] = parameterValue;

				return this;
			}
		}

		private static INavigationParameters _empty;
		private static INavigationParameters _user;
		private static INavigationParameters _closeRegion;

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

		public static INavigationParameters UserNavigation
		{
			get
			{
				if (_user == null)
				{
					_user = new UserNavigationParameters();
				}
				return _user;
			}
		}

		public static INavigationParameters CloseRegion
		{
			get
			{
				if (_closeRegion == null)
				{
					_closeRegion = new CloseRegionNavigationParameters();
				}
				return _closeRegion;
			}
		}

		public static INavigationParameterSetter Create(IDictionary<string, object> parameters)
		{
			return new DictionaryNavigationParameters(parameters);
		}

		public static INavigationParameterSetter Create()
		{
			return new DictionaryNavigationParameters(new Dictionary<string, object>());
		}

		public static bool IsEmptyNavigation(this INavigationParameters parameters)
		{
			return parameters is EmptyNavigationParameters;
		}

		public static bool IsUserNavigation(this INavigationParameters parameters)
		{
			return parameters is UserNavigationParameters;
		}

		public static bool IsCloseRegionNavigation(this INavigationParameters parameters)
		{
			return parameters is CloseRegionNavigationParameters;
		}
	}
}