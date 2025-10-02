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
    public class RegiterUserValiator : AbstractValidator<RegisterUser>
    {
        public RegiterUserValiator()
        {
            RuleFor(o => o.Login).ValidateEmail();

            RuleFor(o => o.Password).ValidatePassword();

            RuleFor(o => o.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(RegisterUserValidationRules.FirstNameMinLength).WithMessage($"Password must be at least {RegisterUserValidationRules.FirstNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.FirstNameMaxLength).WithMessage($"Password cannot exceed {RegisterUserValidationRules.FirstNameMaxLength} characters");

            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(RegisterUserValidationRules.LastNameMinLength).WithMessage($"Password must be at least {RegisterUserValidationRules.LastNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.LastNameMaxLength).WithMessage($"Password cannot exceed {RegisterUserValidationRules.LastNameMaxLength} characters");

            RuleFor(o => o.MiddleName)
                .MinimumLength(RegisterUserValidationRules.MiddleNameMinLength).WithMessage($"Password must be at least {RegisterUserValidationRules.MiddleNameMinLength} characters long")
                .MaximumLength(RegisterUserValidationRules.MiddleNameMaxLength).WithMessage($"Password cannot exceed {RegisterUserValidationRules.MiddleNameMaxLength} characters");
        }
    }
}
