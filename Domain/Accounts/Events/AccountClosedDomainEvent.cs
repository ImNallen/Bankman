using Domain.Abstractions;

namespace Domain.Accounts.Events;

public sealed record AccountClosedDomainEvent(
    AccountId AccountId,
    DateTime OccurredOn) : IDomainEvent;
