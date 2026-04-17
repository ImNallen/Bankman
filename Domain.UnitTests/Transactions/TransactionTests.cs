using Domain.Abstractions;
using Domain.Accounts;
using Domain.Shared;
using Domain.Transactions;
using Domain.Transactions.Events;
using Shouldly;

namespace Domain.UnitTests.Transactions;

public class TransactionTests
{
    private static readonly Currency Usd = Currency.Create("USD").Value;
    private static readonly AccountId Account = AccountId.New();
    private static readonly TransferId Transfer = TransferId.New();

    private static Money Amount(decimal value) => Money.Create(value, Usd).Value;

    [Fact]
    public void Deposit_should_succeed_and_populate_properties()
    {
        TransactionReference reference = TransactionReference.Create("payday").Value;

        Result<Transaction> result = Transaction.Deposit(Account, Amount(100m), reference);

        result.IsSuccess.ShouldBeTrue();
        Transaction tx = result.Value;
        tx.AccountId.ShouldBe(Account);
        tx.Type.ShouldBe(TransactionType.Deposit);
        tx.Amount.Amount.ShouldBe(100m);
        tx.Reference.ShouldBe(reference);
        tx.TransferId.ShouldBeNull();
        tx.OccurredAt.Kind.ShouldBe(DateTimeKind.Utc);
    }

    [Fact]
    public void Deposit_should_allow_null_reference()
    {
        Result<Transaction> result = Transaction.Deposit(Account, Amount(10m));

        result.IsSuccess.ShouldBeTrue();
        result.Value.Reference.ShouldBeNull();
    }

    [Fact]
    public void Withdrawal_should_succeed_and_populate_properties()
    {
        Result<Transaction> result = Transaction.Withdrawal(Account, Amount(25m));

        result.IsSuccess.ShouldBeTrue();
        Transaction tx = result.Value;
        tx.Type.ShouldBe(TransactionType.Withdrawal);
        tx.Amount.Amount.ShouldBe(25m);
        tx.TransferId.ShouldBeNull();
    }

    [Fact]
    public void TransferOut_should_succeed_and_populate_transfer_id()
    {
        Result<Transaction> result = Transaction.TransferOut(Account, Amount(50m), Transfer);

        result.IsSuccess.ShouldBeTrue();
        Transaction tx = result.Value;
        tx.Type.ShouldBe(TransactionType.TransferOut);
        tx.TransferId.ShouldBe(Transfer);
    }

    [Fact]
    public void TransferIn_should_succeed_and_populate_transfer_id()
    {
        Result<Transaction> result = Transaction.TransferIn(Account, Amount(75m), Transfer);

        result.IsSuccess.ShouldBeTrue();
        Transaction tx = result.Value;
        tx.Type.ShouldBe(TransactionType.TransferIn);
        tx.TransferId.ShouldBe(Transfer);
    }

    [Fact]
    public void Each_factory_should_raise_transaction_recorded_event()
    {
        Transaction deposit = Transaction.Deposit(Account, Amount(1m)).Value;
        Transaction withdrawal = Transaction.Withdrawal(Account, Amount(1m)).Value;
        Transaction transferOut = Transaction.TransferOut(Account, Amount(1m), Transfer).Value;
        Transaction transferIn = Transaction.TransferIn(Account, Amount(1m), Transfer).Value;

        AssertEventRaised(deposit, TransactionType.Deposit, expectedTransferId: null);
        AssertEventRaised(withdrawal, TransactionType.Withdrawal, expectedTransferId: null);
        AssertEventRaised(transferOut, TransactionType.TransferOut, expectedTransferId: Transfer);
        AssertEventRaised(transferIn, TransactionType.TransferIn, expectedTransferId: Transfer);
    }

    [Fact]
    public void Factories_should_fail_when_amount_is_null()
    {
        Transaction.Deposit(Account, null!).Error.ShouldBe(TransactionErrors.AmountRequired);
        Transaction.Withdrawal(Account, null!).Error.ShouldBe(TransactionErrors.AmountRequired);
        Transaction.TransferOut(Account, null!, Transfer).Error.ShouldBe(TransactionErrors.AmountRequired);
        Transaction.TransferIn(Account, null!, Transfer).Error.ShouldBe(TransactionErrors.AmountRequired);
    }

    [Fact]
    public void Deposit_should_fail_when_amount_is_zero()
    {
        Result<Transaction> result = Transaction.Deposit(Account, Amount(0m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.AmountNotPositive);
    }

    [Fact]
    public void Withdrawal_should_fail_when_amount_is_zero()
    {
        Result<Transaction> result = Transaction.Withdrawal(Account, Amount(0m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.AmountNotPositive);
    }

    [Fact]
    public void TransferOut_should_fail_when_amount_is_zero()
    {
        Result<Transaction> result = Transaction.TransferOut(Account, Amount(0m), Transfer);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.AmountNotPositive);
    }

    [Fact]
    public void TransferIn_should_fail_when_amount_is_zero()
    {
        Result<Transaction> result = Transaction.TransferIn(Account, Amount(0m), Transfer);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.AmountNotPositive);
    }

    [Fact]
    public void TransferOut_should_fail_when_transfer_id_is_default()
    {
        Result<Transaction> result = Transaction.TransferOut(Account, Amount(10m), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.TransferIdRequired);
    }

    [Fact]
    public void TransferIn_should_fail_when_transfer_id_is_default()
    {
        Result<Transaction> result = Transaction.TransferIn(Account, Amount(10m), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionErrors.TransferIdRequired);
    }

    private static void AssertEventRaised(
        Transaction transaction,
        TransactionType expectedType,
        TransferId? expectedTransferId)
    {
        transaction.DomainEvents.Count.ShouldBe(1);
        TransactionRecordedDomainEvent evt = transaction.DomainEvents
            .ShouldHaveSingleItem()
            .ShouldBeOfType<TransactionRecordedDomainEvent>();

        evt.TransactionId.ShouldBe(transaction.Id);
        evt.AccountId.ShouldBe(transaction.AccountId);
        evt.Type.ShouldBe(expectedType);
        evt.Amount.ShouldBe(transaction.Amount);
        evt.TransferId.ShouldBe(expectedTransferId);
        evt.OccurredOn.ShouldBe(transaction.OccurredAt);
    }
}
