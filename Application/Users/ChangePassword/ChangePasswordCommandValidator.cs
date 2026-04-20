using FluentValidation;

namespace Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.CurrentPassword).NotEmpty();
        RuleFor(c => c.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
