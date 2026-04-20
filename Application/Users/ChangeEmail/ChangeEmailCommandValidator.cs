using FluentValidation;

namespace Application.Users.ChangeEmail;

internal sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.NewEmail).NotEmpty().MaximumLength(320);
    }
}
