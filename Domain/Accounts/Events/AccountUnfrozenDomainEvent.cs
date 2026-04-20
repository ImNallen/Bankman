using Domain.Abstractions;

namespace Domain.Accounts.Events;

public sealed record AccountUnfrozenDomainEvent(
    AccountId AccountId,
    DateTime OccurredOn) : IDomainEvent;
