using Application.Abstractions.Messaging;

namespace Application.Users.UpdateName;

public sealed record UpdateNameCommand(Guid UserId, string FirstName, string LastName) : ICommand;
