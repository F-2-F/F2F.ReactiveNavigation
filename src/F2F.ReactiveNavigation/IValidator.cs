using System;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    public interface IValidator
    {
        IValidationResult Validate(object value);

        /// <summary>
        /// Indicates whether this validator can validate the given object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanValidate(object value);
    }

    public interface IValidator<in T> : IValidator
    {
        IValidationResult Validate(T value);
    }
}
