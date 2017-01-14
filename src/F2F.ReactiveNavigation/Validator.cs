using System;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    public abstract class Validator<T> : IValidator<T>
    {
        bool IValidator.CanValidate(object value)
        {
            return value is T;
        }

        IValidationResult IValidator.Validate(object value)
        {
            if (!((IValidator)this).CanValidate(value))
                throw new InvalidOperationException("Given object cannot be validated by this validator.");

            return Validate((T)value);
        }

        public abstract IValidationResult Validate(T value);
    }
}
