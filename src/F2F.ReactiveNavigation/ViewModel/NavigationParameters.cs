using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation.ViewModel
{
	public static class NavigationParameters
	{
		private class EmptyNavigationParameters : F2F.ReactiveNavigation.ViewModel.INavigationParameters
		{
			public T Get<T>(string parameterName)
			{
				return default(T);
			}

			public void Set<T>(string parameterName, T value)
			{
			}
		}

		private class DictionaryNavigationParameters : F2F.ReactiveNavigation.ViewModel.INavigationParameters
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

		private static F2F.ReactiveNavigation.ViewModel.INavigationParameters _empty;

		public static F2F.ReactiveNavigation.ViewModel.INavigationParameters Empty
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

		public static F2F.ReactiveNavigation.ViewModel.INavigationParameters Create(IDictionary<string, object> parameters)
		{
			return new DictionaryNavigationParameters(parameters);
		}

		public static F2F.ReactiveNavigation.ViewModel.INavigationParameters Create()
		{
			return new DictionaryNavigationParameters(new Dictionary<string, object>());
		}

		public static F2F.ReactiveNavigation.ViewModel.INavigationParameters UserNavigation()
		{
			return new UserNavigationParameters();
		}

		public static bool IsUserNavigation(this F2F.ReactiveNavigation.ViewModel.INavigationParameters parameters)
		{
			return parameters is UserNavigationParameters;
		}
	}
}