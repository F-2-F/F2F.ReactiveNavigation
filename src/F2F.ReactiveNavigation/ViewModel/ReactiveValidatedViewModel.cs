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
using System.Threading.Tasks;

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

        private bool _hasErrors;
        private bool _isValid;

        private ValidationResult _validationResults = new ValidationSuccess();
        private readonly Subject<ValidationResult> _validationSubject = new Subject<ValidationResult>();

        protected internal override async Task Initialize()
        {
            await base.Initialize();

            this.Changed
                .Where(x => x.PropertyName != "HasErrors" && x.PropertyName != "IsValid")
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(_ => Validate())
                .Subscribe();
            
            this.RaisePropertyChanged("ValidationObservable");  // this cheaply triggers an initial validation
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            return _validationResults.GetErrorsFor(propertyName);
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            set { this.RaiseAndSetIfChanged(ref _hasErrors, value); }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set { this.RaiseAndSetIfChanged(ref _isValid, value); }
        }

        public IObservable<ValidationResult> ValidationObservable
        {
            get { return _validationSubject; }
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
                    .Union(_validationResults.GetPropertiesInError())
                    .Distinct();

            // HasErrors must be true before raising the ErrorsChanged event, otherwise WPF will ignore the errors !!
            // Thx to http://stackoverflow.com/a/24837028 for this hint ;-)
            IsValid = _validationResults.IsValid;
            HasErrors = !IsValid;

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