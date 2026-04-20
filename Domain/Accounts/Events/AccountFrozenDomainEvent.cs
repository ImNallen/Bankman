using Domain.Abstractions;

namespace Domain.Accounts.Events;

public sealed record AccountFrozenDomainEvent(
    AccountId AccountId,
    DateTime OccurredOn) : IDomainEvent;
