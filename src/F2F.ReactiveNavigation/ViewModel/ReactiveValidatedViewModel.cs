using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace F2F.ReactiveNavigation.ViewModel
{
	
	public class ReactiveValidatedViewModel : ReactiveViewModel, INotifyDataErrorInfo
	{
		/// <summary>
		/// "Empty" validator whose single purpose is to return a positive validation result.
		/// </summary>
		private class AlwaysValidValidator : AbstractValidator<object>
		{
		}

		/// <summary>
		/// ValidationResult that always returns success
		/// </summary>
		private class ValidationSuccess : ValidationResult
		{
			public ValidationSuccess()
			{
			}

			public ValidationSuccess(System.Collections.Generic.IEnumerable<ValidationFailure> failures)
				: base()
			{
			}

			public override bool IsValid
			{
				get { return true; }
			}
		}


		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private ObservableAsPropertyHelper<bool> _hasErrors;
		private ValidationResult _validationResults = new ValidationSuccess();

		protected override void Init()
		{
			
		}

		public System.Collections.IEnumerable GetErrors(string propertyName)
		{
			throw new NotImplementedException();
		}

		public bool HasErrors
		{
			get { return _hasErrors.Value; }
		}

		protected bool Validate()
		{
			var previousResults = _validationResults;
			var validator = ProvideValidator();

			_validationResults = validator.Validate(this);
			
			// raise errors changed event for union set of previous and current results
			// so a change notification is raised for each affected property
			var propertiesInError = 
				previousResults
					.GetPropertiesInError()
					.Union(_validationResults.GetPropertiesInError());

			foreach(var property in propertiesInError)
				RaiseErrorsChanged(property);

			return _validationResults.IsValid;
		}

		protected virtual void RaiseErrorsChanged(string propertyName)
		{
			EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
			if (handler != null)
				handler(this, new DataErrorsChangedEventArgs(propertyName));
		}

		protected virtual void RaiseErrorsChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			RaiseErrorsChanged(PropertyName.Of(projection));
		}

		protected virtual IValidator ProvideValidator()
		{
			return new AlwaysValidValidator();
		}
		
	}
}
