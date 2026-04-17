using Domain.Abstractions;
using Domain.Shared;

namespace Domain.Accounts.Events;

public sealed record AccountCreditedDomainEvent(
    AccountId AccountId,
    Money Amount,
    Money NewBalance,
    DateTime OccurredOn) : IDomainEvent;
