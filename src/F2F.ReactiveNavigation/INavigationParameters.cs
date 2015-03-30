using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
	// need fluent builder for this, e.g.
	// NavigationParameters.Add("moep", 123).Add("bloep", "blubb").Create();
	// or maybe we can use collection intializer syntax with a ctor parameter of type IEnuermable<Param>
	
	// TODO: make this type dynamic and get rid of this stupid string based generic shit
	public interface INavigationParameters
	{
		T Get<T>(string parameterName);

		void Set<T>(string parameterName, T value);
	}
}