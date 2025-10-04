using Application.Models;
using Application.Validators.Rules;
using FluentValidation;

namespace Application.Validators
{
    public class CreateWorkGroupValidator : AbstractValidator<CreateWorkGroup>
    {
        public CreateWorkGroupValidator() 
        {
            RuleFor(o => o.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(CreateWorkGroupValidationRules.NameMinLength).WithMessage($"Name must be at least {CreateWorkGroupValidationRules.NameMinLength} characters long")
                .MaximumLength(CreateWorkGroupValidationRules.NameMaxLength).WithMessage($"Name cannot exceed {CreateWorkGroupValidationRules.NameMaxLength} characters");
        }
    }
}
