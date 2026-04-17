using Domain.Abstractions;
using Domain.Accounts;
using Domain.Shared;
using Domain.Transactions.Events;

namespace Domain.Transactions;

public sealed class Transaction : AggregateRoot<TransactionId>
{
    private Transaction(
        TransactionId id,
        AccountId accountId,
        TransactionType type,
        Money amount,
        TransactionReference? reference,
        TransferId? transferId,
        DateTime occurredAt) : base(id)
    {
        AccountId = accountId;
        Type = type;
        Amount = amount;
        Reference = reference;
        TransferId = transferId;
        OccurredAt = occurredAt;
    }

    public AccountId AccountId { get; }

    public TransactionType Type { get; }

    public Money Amount { get; }

    public TransactionReference? Reference { get; }

    public TransferId? TransferId { get; }

    public DateTime OccurredAt { get; }

    public static Result<Transaction> Deposit(
        AccountId accountId,
        Money amount,
        TransactionReference? reference = null) =>
        Create(accountId, TransactionType.Deposit, amount, transferId: null, reference);

    public static Result<Transaction> Withdrawal(
        AccountId accountId,
        Money amount,
        TransactionReference? reference = null) =>
        Create(accountId, TransactionType.Withdrawal, amount, transferId: null, reference);

    public static Result<Transaction> TransferOut(
        AccountId accountId,
        Money amount,
        TransferId transferId,
        TransactionReference? reference = null) =>
        Create(accountId, TransactionType.TransferOut, amount, transferId, reference);

    public static Result<Transaction> TransferIn(
        AccountId accountId,
        Money amount,
        TransferId transferId,
        TransactionReference? reference = null) =>
        Create(accountId, TransactionType.TransferIn, amount, transferId, reference);

    private static Result<Transaction> Create(
        AccountId accountId,
        TransactionType type,
        Money amount,
        TransferId? transferId,
        TransactionReference? reference)
    {
        if (amount is null)
        {
            return Result.Failure<Transaction>(TransactionErrors.AmountRequired);
        }

        if (amount.Amount <= 0m)
        {
            return Result.Failure<Transaction>(TransactionErrors.AmountNotPositive);
        }

        bool isTransfer = type is TransactionType.TransferIn or TransactionType.TransferOut;

        if (isTransfer && (transferId is null || transferId.Value.Value == Guid.Empty))
        {
            return Result.Failure<Transaction>(TransactionErrors.TransferIdRequired);
        }

        if (!isTransfer && transferId is not null)
        {
            return Result.Failure<Transaction>(TransactionErrors.TransferIdNotAllowed);
        }

        DateTime occurredAt = DateTime.UtcNow;

        var transaction = new Transaction(
            TransactionId.New(),
            accountId,
            type,
            amount,
            reference,
            transferId,
            occurredAt);

        transaction.RaiseDomainEvent(new TransactionRecordedDomainEvent(
            transaction.Id,
            transaction.AccountId,
            transaction.Type,
            transaction.Amount,
            transaction.TransferId,
            occurredAt));

        return transaction;
    }
}
