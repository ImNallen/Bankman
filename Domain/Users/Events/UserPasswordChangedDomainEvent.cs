using Domain.Abstractions;

namespace Domain.Users.Events;

public sealed record UserPasswordChangedDomainEvent(
    UserId UserId,
    DateTime OccurredOn) : IDomainEvent;
