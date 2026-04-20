using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Application.Users.UpdateName;

internal sealed class UpdateNameCommandHandler : ICommandHandler<UpdateNameCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNameCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateNameCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        Result<PersonName> nameResult = PersonName.Create(request.FirstName, request.LastName);
        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        Result updateResult = user.UpdateName(nameResult.Value);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
