using FluentValidation;

namespace Application.Users.UpdateName;

internal sealed class UpdateNameCommandValidator : AbstractValidator<UpdateNameCommand>
{
    public UpdateNameCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(100);
    }
}
