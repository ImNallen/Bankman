using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        if (!_passwordHasher.Verify(user.PasswordHash.Value, request.CurrentPassword))
        {
            return Result.Failure(UserErrors.IncorrectPassword);
        }

        Result<PasswordHash> newHashResult = PasswordHash.Create(_passwordHasher.Hash(request.NewPassword));
        if (newHashResult.IsFailure)
        {
            return Result.Failure(newHashResult.Error);
        }

        Result changeResult = user.ChangePassword(newHashResult.Value);
        if (changeResult.IsFailure)
        {
            return changeResult;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
