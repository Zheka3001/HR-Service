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
                .MinimumLength(RegisterUserValidationRules.FirstNameMinLength).WithMessage($"First name must be at least {RegisterUserValidationRules.FirstNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.FirstNameMaxLength).WithMessage($"First name cannot exceed {RegisterUserValidationRules.FirstNameMaxLength} characters");

            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(RegisterUserValidationRules.LastNameMinLength).WithMessage($"Last name must be at least {RegisterUserValidationRules.LastNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.LastNameMaxLength).WithMessage($"Last name cannot exceed {RegisterUserValidationRules.LastNameMaxLength} characters");

            RuleFor(o => o.MiddleName)
                .MinimumLength(RegisterUserValidationRules.MiddleNameMinLength).WithMessage($"First name must be at least {RegisterUserValidationRules.MiddleNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.MiddleNameMaxLength).WithMessage($"First name cannot exceed {RegisterUserValidationRules.MiddleNameMaxLength} characters");

            RuleFor(o => o.WorkGroupId)
                .Must(workGroupId => workGroupId > 0)
                .WithMessage("Work group id must be valid positive integer");
        }
    }
}
