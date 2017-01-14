using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation.ViewModel
{
    public interface INavigationParameterSetter : INavigationParameters
    {
        INavigationParameterSetter Add<T>(string parameterName, T parameterValue);
    }
}