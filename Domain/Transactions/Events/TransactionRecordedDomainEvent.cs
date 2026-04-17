using Domain.Abstractions;
using Domain.Accounts;
using Domain.Shared;

namespace Domain.Transactions.Events;

public sealed record TransactionRecordedDomainEvent(
    TransactionId TransactionId,
    AccountId AccountId,
    TransactionType Type,
    Money Amount,
    TransferId? TransferId,
    DateTime OccurredOn) : IDomainEvent;
