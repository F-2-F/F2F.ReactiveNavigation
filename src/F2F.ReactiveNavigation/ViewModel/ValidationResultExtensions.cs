using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentValidation.Results
{
	public static class ValidationResultExtensions
	{
		public static IEnumerable<string> GetPropertiesInError(this ValidationResult self)
		{
			return self != null
					? self.Errors.Select(f => f.PropertyName)
					: null;
		}

		public static IEnumerable<string> GetErrorsFor(this ValidationResult self, string propertyName)
		{
			return self != null
					? self.Errors.Where(f => f.PropertyName.Equals(propertyName)).Select(f => f.ErrorMessage)
					: null;
		}
	}
}
