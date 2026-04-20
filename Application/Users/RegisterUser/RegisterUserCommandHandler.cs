using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        Result<PersonName> nameResult = PersonName.Create(request.FirstName, request.LastName);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Guid>(nameResult.Error);
        }

        User? existing = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<Guid>(UserErrors.EmailAlreadyInUse);
        }

        Result<PasswordHash> passwordResult = PasswordHash.Create(_passwordHasher.Hash(request.Password));
        if (passwordResult.IsFailure)
        {
            return Result.Failure<Guid>(passwordResult.Error);
        }

        Result<User> userResult = User.Register(emailResult.Value, passwordResult.Value, nameResult.Value);
        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        await _userRepository.AddAsync(userResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return userResult.Value.Id.Value;
    }
}
