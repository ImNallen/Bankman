using Domain.Abstractions;
using Domain.Shared;

namespace Domain.Users.Events;

public sealed record UserEmailChangedDomainEvent(
    UserId UserId,
    Email NewEmail,
    DateTime OccurredOn) : IDomainEvent;
