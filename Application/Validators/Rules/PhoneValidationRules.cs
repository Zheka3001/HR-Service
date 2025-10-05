using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rules
{
    public static class PhoneValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidatePhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format");
        }
    }
}
