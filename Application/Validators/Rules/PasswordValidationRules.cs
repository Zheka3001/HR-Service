using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rules
{
    public static class PasswordValidationRules
    {
        public const int PasswordMaxLength = 50;
        public const int PasswordMinLength = 5;

        public static IRuleBuilderOptions<T, string> ValidatePassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(PasswordMinLength).WithMessage($"Password must be at least {PasswordMinLength} characters long")
                .MaximumLength(PasswordMaxLength).WithMessage($"Password cannot exceed {PasswordMaxLength} characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one digit")
                .Matches(@"[\W]").WithMessage("Password must contain at least one special character");
        }
    }
}
