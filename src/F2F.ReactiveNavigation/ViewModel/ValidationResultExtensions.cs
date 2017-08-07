using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    public static class ValidationResultExtensions
    {
        public static IEnumerable<string> GetPropertiesInError(this IValidationResult self)
        {
            return self != null
                    ? self.Errors.Select(f => f.PropertyName)
                    : null;
        }

        public static IEnumerable<string> GetErrorsFor(this IValidationResult self, string propertyName)
        {
            return self != null
                    ? self.Errors.Where(f => f.PropertyName.Equals(propertyName)).Select(f => f.ErrorMessage)
                    : null;
        }
    }
}
