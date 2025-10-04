using Application.Models;
using FluentValidation;

namespace Application.Validators
{
    public class MoveHrsRequestValidator : AbstractValidator<MoveHrsRequest>
    {
        public MoveHrsRequestValidator()
        {
            RuleFor(o => o.WorkGroupId)
                .Must(workGroupId => workGroupId > 0)
                .WithMessage("Work group id must be valid positive integer");

            RuleFor(o => o.UserIds)
                .Must(userIds => userIds.All(x => x > 0))
                .WithMessage("User ids must be valid positive integers");
        }
    }
}
