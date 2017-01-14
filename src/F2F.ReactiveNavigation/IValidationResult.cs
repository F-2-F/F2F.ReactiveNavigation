using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2F.ReactiveNavigation
{
    /// <summary>
    /// Represents result of a validation performed by an <see cref="IValidator"/>.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Zero to n validation errors that occured during validation.
        /// </summary>
        IEnumerable<IValidationError> Errors { get; }

        /// <summary>
        /// Indication, whether validation has passed. Should only return true, if
        /// <see cref="Errors"/> yields no errors.
        /// </summary>
        bool IsValid { get; }
    }
}
