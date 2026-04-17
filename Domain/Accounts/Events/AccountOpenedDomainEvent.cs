using Domain.Abstractions;
using Domain.Shared;
using Domain.Users;

namespace Domain.Accounts.Events;

public sealed record AccountOpenedDomainEvent(
    AccountId AccountId,
    UserId OwnerId,
    AccountNumber Number,
    Currency Currency,
    DateTime OccurredOn) : IDomainEvent;
