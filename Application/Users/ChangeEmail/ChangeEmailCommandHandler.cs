using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Application.Users.ChangeEmail;

internal sealed class ChangeEmailCommandHandler : ICommandHandler<ChangeEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        UserId userId = UserId.From(request.UserId);

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        Result<Email> emailResult = Email.Create(request.NewEmail);
        if (emailResult.IsFailure)
        {
            return Result.Failure(emailResult.Error);
        }

        User? existing = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (existing is not null && existing.Id != userId)
        {
            return Result.Failure(UserErrors.EmailAlreadyInUse);
        }

        Result changeResult = user.ChangeEmail(emailResult.Value);
        if (changeResult.IsFailure)
        {
            return changeResult;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
