using FluentValidation;
using FluentValidation.Results;

namespace Application.Services
{
    public class ValidationService : IValidationService
    {
        private readonly Lazy<IEnumerable<IValidator>> _validators;

        public ValidationService(Lazy<IEnumerable<IValidator>> validators)
        {
            _validators = validators;
        }

        public async Task ValidateAsync<T>(T modelToValidate) where T : class
        {
            if (modelToValidate == null)
            {
                throw new ArgumentNullException(nameof(modelToValidate));
            }

            var matchedValidators = _validators.Value.Where(v => v is IValidator<T>)
                .Cast<IValidator<T>>().ToArray();

            if (!matchedValidators.Any())
            {
                throw new ArgumentException("No validators found", nameof(T));
            }

            var validationResults = await Task.WhenAll(matchedValidators.Select(v => v.ValidateAsync(modelToValidate)));

            var firstInvalidResult = validationResults.FirstOrDefault(result => result is { IsValid: false });

            if (firstInvalidResult != null)
            {
                throw new ValidationException(
                    firstInvalidResult.Errors.Select(
                        error => new ValidationFailure(error.PropertyName, error.ErrorMessage)));
            }
        }
    }
}
