using Application.Models;
using Application.Validators.Rules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class CreateApplicantRequestValidator : AbstractValidator<CreateApplicantRequest>
    {
        public CreateApplicantRequestValidator(IValidator<CreateSocialNetworkInfoRequest> createSocialNetworkInfoRequestValidator)
        {
            RuleFor(o => o.Email).ValidateEmail();

            RuleFor(o => o.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(CreateApplicantValidationRules.FirstNameMinLength).WithMessage($"First name must be at least {CreateApplicantValidationRules.FirstNameMinLength} characters long")
                .MaximumLength(CreateApplicantValidationRules.FirstNameMaxLength).WithMessage($"First name cannot exceed {CreateApplicantValidationRules.FirstNameMaxLength} characters");

            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(CreateApplicantValidationRules.LastNameMinLength).WithMessage($"Last name must be at least {CreateApplicantValidationRules.LastNameMinLength} characters long")
                .MaximumLength(CreateApplicantValidationRules.LastNameMaxLength).WithMessage($"Last name cannot exceed {CreateApplicantValidationRules.LastNameMaxLength} characters");

            RuleFor(o => o.MiddleName)
                .MinimumLength(CreateApplicantValidationRules.MiddleNameMinLength).WithMessage($"Middle name must be at least {CreateApplicantValidationRules.MiddleNameMinLength} characters long")
                .MaximumLength(CreateApplicantValidationRules.MiddleNameMaxLength).WithMessage($"Middle name cannot exceed {CreateApplicantValidationRules.MiddleNameMaxLength} characters");
    
            RuleFor(o => o.PhoneNumber).ValidatePhoneNumber();

            RuleFor(o => o.Country).ValidateCountryName();

            RuleFor(o => o.DateOfBirth).ValidateDateOfBirth();

            RuleForEach(o => o.CreateSocialNetworkInfoRequests).SetValidator(createSocialNetworkInfoRequestValidator);
        }
    }

    public class CreateSocialNetworkInfoRequestValidator : AbstractValidator<CreateSocialNetworkInfoRequest>
    {
        public CreateSocialNetworkInfoRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Social network username is required");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid social network type");
        }
    }
}
