using System;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    public class ValidationError : IValidationError
    {
        public ValidationError(string propertyName = null, string errorMessage = null)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public string PropertyName { get; }
    }
}
