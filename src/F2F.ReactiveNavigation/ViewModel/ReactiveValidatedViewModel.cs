using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;

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

			public override bool IsValid
			{
				get { return true; }
			}
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		private ObservableAsPropertyHelper<bool> _hasErrors;
		private ObservableAsPropertyHelper<bool> _isValid;

		private ValidationResult _validationResults = new ValidationSuccess();
		private readonly Subject<ValidationResult> _validationSubject = new Subject<ValidationResult>();

		protected internal override void Initialize()
		{
			base.Initialize();

			_hasErrors =
				this.Changed
					.Where(x => x.PropertyName != "HasErrors" && x.PropertyName != "IsValid")
					.ObserveOn(RxApp.MainThreadScheduler)
					.Select(_ => !Validate())
					.ToProperty(this, x => x.HasErrors);

			_isValid =
				this.WhenAnyValue(x => x.HasErrors)
					.Select(x => !x)
					.ToProperty(this, x => x.IsValid);

			this.RaisePropertyChanged("ValidationObservable");	// this cheaply triggers an initial validation
		}

		public IEnumerable GetErrors(string propertyName)
		{
			if (String.IsNullOrEmpty(propertyName))
				return Enumerable.Empty<string>();

			return _validationResults.GetErrorsFor(propertyName);
		}

		public bool HasErrors
		{
			get { return _hasErrors != null ? _hasErrors.Value : false; }
		}

		public bool IsValid
		{
			get { return _isValid != null ? _isValid.Value : true; }
		}

		public IObservable<ValidationResult> ValidationObservable
		{
			get { return _validationSubject; }
		}

		private bool Validate()
		{
			var previousResults = _validationResults;
			var validator = ProvideValidator();

			_validationResults = validator.Validate(this);

			// raise errors changed event for union set of previous and current results
			// so a change notification is raised for each affected property
			var propertiesInError =
				previousResults
					.GetPropertiesInError()
					.Union(_validationResults.GetPropertiesInError())
					.Distinct();

			foreach (var property in propertiesInError)
				RaiseErrorsChanged(property);

			_validationSubject.OnNext(_validationResults);
			return _validationResults.IsValid;
		}

		private void RaiseErrorsChanged(string propertyName)
		{
			var handler = ErrorsChanged;
			if (handler != null)
				handler(this, new DataErrorsChangedEventArgs(propertyName));
		}

		protected virtual IValidator ProvideValidator()
		{
			return new AlwaysValidValidator();
		}
	}
}