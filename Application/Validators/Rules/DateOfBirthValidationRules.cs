using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rules
{
    public static class DateOfBirthValidationRules
    {
        public static IRuleBuilderOptions<T, DateTime> ValidateDateOfBirth<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Date of Birth is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date of Birth cannot be in the future")
                .Must(BeWithValidAgeRange).WithMessage("Age must be beetween 18 and 120 years");
        }

        private static bool BeWithValidAgeRange(DateTime dateOfBirth)
        {
            var age = DateTime.UtcNow.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            return age >= 18 && age <= 120;
        }
    }
}
