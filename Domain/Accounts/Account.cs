using Domain.Abstractions;
using Domain.Accounts.Events;
using Domain.Shared;
using Domain.Users;

namespace Domain.Accounts;

public sealed class Account : AggregateRoot<AccountId>
{
    private Account()
    {
    }

    private Account(
        AccountId id,
        UserId ownerId,
        AccountNumber number,
        Money balance,
        AccountStatus status,
        DateTime openedAt) : base(id)
    {
        OwnerId = ownerId;
        Number = number;
        Balance = balance;
        Status = status;
        OpenedAt = openedAt;
    }

    public UserId OwnerId { get; private set; }

    public AccountNumber Number { get; private set; }

    public Money Balance { get; private set; }

    public AccountStatus Status { get; private set; }

    public DateTime OpenedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    public static Result<Account> Open(UserId owner, AccountNumber number, Currency currency)
    {
        if (owner == default)
        {
            return Result.Failure<Account>(AccountErrors.OwnerRequired);
        }

        if (number is null)
        {
            return Result.Failure<Account>(AccountErrors.AccountNumberRequired);
        }

        if (currency is null)
        {
            return Result.Failure<Account>(AccountErrors.CurrencyRequired);
        }

        DateTime openedAt = DateTime.UtcNow;
        var account = new Account(
            AccountId.New(),
            owner,
            number,
            Money.Zero(currency),
            AccountStatus.Active,
            openedAt);

        account.RaiseDomainEvent(new AccountOpenedDomainEvent(
            account.Id,
            owner,
            number,
            currency,
            openedAt));

        return account;
    }

    public Result Credit(Money amount)
    {
        Result validation = ValidateMutation(amount);
        if (validation.IsFailure)
        {
            return validation;
        }

        Result<Money> newBalance = Balance.Add(amount);
        if (newBalance.IsFailure)
        {
            return Result.Failure(newBalance.Error);
        }

        Balance = newBalance.Value;
        RaiseDomainEvent(new AccountCreditedDomainEvent(Id, amount, Balance, DateTime.UtcNow));
        return Result.Success();
    }

    public Result Debit(Money amount)
    {
        Result validation = ValidateMutation(amount);
        if (validation.IsFailure)
        {
            return validation;
        }

        if (Balance.Amount < amount.Amount)
        {
            return Result.Failure(AccountErrors.InsufficientFunds);
        }

        Result<Money> newBalance = Balance.Subtract(amount);
        if (newBalance.IsFailure)
        {
            return Result.Failure(newBalance.Error);
        }

        Balance = newBalance.Value;
        RaiseDomainEvent(new AccountDebitedDomainEvent(Id, amount, Balance, DateTime.UtcNow));
        return Result.Success();
    }

    public Result Freeze()
    {
        if (Status == AccountStatus.Closed)
        {
            return Result.Failure(AccountErrors.AlreadyClosed);
        }

        if (Status == AccountStatus.Frozen)
        {
            return Result.Failure(AccountErrors.AlreadyFrozen);
        }

        Status = AccountStatus.Frozen;
        RaiseDomainEvent(new AccountFrozenDomainEvent(Id, DateTime.UtcNow));
        return Result.Success();
    }

    public Result Unfreeze()
    {
        if (Status == AccountStatus.Closed)
        {
            return Result.Failure(AccountErrors.AlreadyClosed);
        }

        if (Status != AccountStatus.Frozen)
        {
            return Result.Failure(AccountErrors.NotFrozen);
        }

        Status = AccountStatus.Active;
        RaiseDomainEvent(new AccountUnfrozenDomainEvent(Id, DateTime.UtcNow));
        return Result.Success();
    }

    public Result Close()
    {
        if (Status == AccountStatus.Closed)
        {
            return Result.Failure(AccountErrors.AlreadyClosed);
        }

        if (Balance.Amount != 0m)
        {
            return Result.Failure(AccountErrors.HasRemainingBalance);
        }

        DateTime closedAt = DateTime.UtcNow;
        Status = AccountStatus.Closed;
        ClosedAt = closedAt;
        RaiseDomainEvent(new AccountClosedDomainEvent(Id, closedAt));
        return Result.Success();
    }

    private Result ValidateMutation(Money amount)
    {
        if (amount is null)
        {
            return Result.Failure(AccountErrors.AmountRequired);
        }

        if (Status != AccountStatus.Active)
        {
            return Result.Failure(AccountErrors.NotActive);
        }

        if (amount.Currency != Balance.Currency)
        {
            return Result.Failure(AccountErrors.CurrencyMismatch);
        }

        if (amount.Amount <= 0)
        {
            return Result.Failure(AccountErrors.AmountMustBePositive);
        }

        return Result.Success();
    }
}
