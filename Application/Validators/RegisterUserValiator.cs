using Application.Models;
using Application.Validators.Rules;
using FluentValidation;

namespace Application.Validators
{
    public class RegisterUserValiator : AbstractValidator<RegisterUser>
    {
        public RegisterUserValiator()
        {
            RuleFor(o => o.Login).ValidateEmail();

            RuleFor(o => o.Password).ValidatePassword();

            RuleFor(o => o.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(FullNameValidationRules.FirstNameMinLength).WithMessage($"First name must be at least {FullNameValidationRules.FirstNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.FirstNameMaxLength).WithMessage($"First name cannot exceed {FullNameValidationRules.FirstNameMaxLength} characters");

            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(FullNameValidationRules.LastNameMinLength).WithMessage($"Last name must be at least {FullNameValidationRules.LastNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.LastNameMaxLength).WithMessage($"Last name cannot exceed {FullNameValidationRules.LastNameMaxLength} characters");

            RuleFor(o => o.MiddleName)
                .MinimumLength(FullNameValidationRules.MiddleNameMinLength).WithMessage($"First name must be at least {FullNameValidationRules.MiddleNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.MiddleNameMaxLength).WithMessage($"First name cannot exceed {FullNameValidationRules.MiddleNameMaxLength} characters");

            RuleFor(o => o.WorkGroupId)
                .Must(workGroupId => workGroupId > 0)
                .WithMessage("Work group id must be valid positive integer");
        }
    }
}
