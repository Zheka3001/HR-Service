using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rules
{
    public static class CountryValidationRules
    {
        public const int CountryNameMaxLength = 100;

        public static IRuleBuilderOptions<T, string> ValidateCountryName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(CountryNameMaxLength).WithMessage($"Country name cannot exceed {CountryNameMaxLength} characters");
        }
    }
}
