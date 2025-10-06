using Application.Models;
using Application.Validators.Rules;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateApplicantRequestValidator : AbstractValidator<UpdateApplicantRequest>
    {
        public UpdateApplicantRequestValidator(IValidator<SocialNetwork> socialNetworkValidator)
        {
            RuleFor(o => o.Email).ValidateEmail();

            RuleFor(o => o.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(FullNameValidationRules.FirstNameMinLength).WithMessage($"First name must be at least {FullNameValidationRules.FirstNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.FirstNameMaxLength).WithMessage($"First name cannot exceed {FullNameValidationRules.FirstNameMaxLength} characters");

            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(FullNameValidationRules.LastNameMinLength).WithMessage($"Last name must be at least {FullNameValidationRules.LastNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.LastNameMaxLength).WithMessage($"Last name cannot exceed {FullNameValidationRules.LastNameMaxLength} characters");

            RuleFor(o => o.MiddleName)
                .MinimumLength(FullNameValidationRules.MiddleNameMinLength).WithMessage($"Middle name must be at least {FullNameValidationRules.MiddleNameMinLength} characters long")
                .MaximumLength(FullNameValidationRules.MiddleNameMaxLength).WithMessage($"Middle name cannot exceed {FullNameValidationRules.MiddleNameMaxLength} characters");

            RuleFor(o => o.PhoneNumber).ValidatePhoneNumber();

            RuleFor(o => o.Country).ValidateCountryName();

            RuleFor(o => o.DateOfBirth).ValidateDateOfBirth();

            RuleForEach(o => o.SocialNetworks).SetValidator(socialNetworkValidator);
        }
    }
}
