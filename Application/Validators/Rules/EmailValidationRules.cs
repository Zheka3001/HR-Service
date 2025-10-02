using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rules
{
    public static class EmailValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidateEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
