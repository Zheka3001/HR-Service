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
    public class LoginCredentialsValidator : AbstractValidator<LoginRequest>
    {
        public LoginCredentialsValidator()
        {
            RuleFor(o => o.Email).ValidateEmail();
            RuleFor(o => o.Password).ValidatePassword();
        }
    }
}
