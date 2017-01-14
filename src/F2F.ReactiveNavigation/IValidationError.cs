using System;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    /// <summary>
    /// Represents a single validation error. It may optionally carry a property name and error message.
    /// </summary>
    public interface IValidationError
    {
        string PropertyName { get; }

        string ErrorMessage { get; }
    }
}
