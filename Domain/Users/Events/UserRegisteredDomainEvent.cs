using Domain.Abstractions;
using Domain.Shared;

namespace Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(
    UserId UserId,
    Email Email,
    DateTime OccurredOn) : IDomainEvent;
