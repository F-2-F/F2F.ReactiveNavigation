using System;
using System.Collections.Generic;
using System.Linq;

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

			public bool Has(string parameterName)
			{
				return _parameters.ContainsKey(parameterName);
			}
		}

		private static Lazy<INavigationParameters> _empty = new Lazy<INavigationParameters>(() => new EmptyNavigationParameters());
		private static Lazy<INavigationParameters> _user = new Lazy<INavigationParameters>(() => new UserNavigationParameters());
		private static Lazy<INavigationParameters> _closeRegion = new Lazy<INavigationParameters>(() => new CloseRegionNavigationParameters());

		public static INavigationParameters Empty
		{
			get { return _empty.Value; }
		}

		public static INavigationParameters UserNavigation
		{
			get { return _user.Value; }
		}

		public static INavigationParameters CloseRegion
		{
			get { return _closeRegion.Value; }
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