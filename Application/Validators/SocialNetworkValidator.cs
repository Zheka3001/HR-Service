using Application.Models;
using FluentValidation;

namespace Application.Validators
{
    public class SocialNetworkValidator : AbstractValidator<SocialNetwork>
    {
        public SocialNetworkValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Social network username is required");
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid social network type");
        }
    }
}
