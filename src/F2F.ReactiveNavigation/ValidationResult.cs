using System;
using System.Collections.Generic;
using System.Linq;

namespace F2F.ReactiveNavigation
{
    public class ValidationResult : IValidationResult
    {
        /// <summary>
        /// Validation result with no errors, so <see cref="IsValid"/> returns true.
        /// </summary>
        public static readonly IValidationResult Success = new ValidationResult();

        public IEnumerable<IValidationError> Errors { get; }

        public bool IsValid { get; }

        public ValidationResult()
            : this(null)
        {
        }

        public ValidationResult(IEnumerable<IValidationError> errors)
        {
            Errors = errors ?? Enumerable.Empty<IValidationError>();
            IsValid = !Errors.Any();
        }
    }
}
