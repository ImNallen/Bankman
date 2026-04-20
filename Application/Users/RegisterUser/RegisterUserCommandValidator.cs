using FluentValidation;

namespace Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().MaximumLength(320);
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
