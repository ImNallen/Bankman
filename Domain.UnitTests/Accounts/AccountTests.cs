using Domain.Abstractions;
using Domain.Accounts;
using Domain.Accounts.Events;
using Domain.Shared;
using Domain.Users;
using Shouldly;

namespace Domain.UnitTests.Accounts;

public class AccountTests
{
    private static Currency Usd => Currency.Create("USD").Value;
    private static Currency Eur => Currency.Create("EUR").Value;

    private static AccountNumber Iban => AccountNumber.Create("DE89370400440532013000").Value;

    private static Money Money(decimal amount, Currency? currency = null) =>
        Domain.Shared.Money.Create(amount, currency ?? Usd).Value;

    private static Account OpenActive(Currency? currency = null) =>
        Account.Open(UserId.New(), Iban, currency ?? Usd).Value;

    [Fact]
    public void Open_should_succeed_with_valid_inputs()
    {
        var owner = UserId.New();
        DateTime before = DateTime.UtcNow;

        Result<Account> result = Account.Open(owner, Iban, Usd);

        result.IsSuccess.ShouldBeTrue();
        Account account = result.Value;
        account.OwnerId.ShouldBe(owner);
        account.Number.ShouldBe(Iban);
        account.Balance.Amount.ShouldBe(0m);
        account.Balance.Currency.ShouldBe(Usd);
        account.Status.ShouldBe(AccountStatus.Active);
        account.OpenedAt.ShouldBeGreaterThanOrEqualTo(before);
        account.ClosedAt.ShouldBeNull();
    }

    [Fact]
    public void Open_should_raise_AccountOpenedDomainEvent()
    {
        var owner = UserId.New();

        Account account = Account.Open(owner, Iban, Usd).Value;

        account.DomainEvents.Count.ShouldBe(1);
        var evt = account.DomainEvents.First().ShouldBeOfType<AccountOpenedDomainEvent>();
        evt.AccountId.ShouldBe(account.Id);
        evt.OwnerId.ShouldBe(owner);
        evt.Number.ShouldBe(Iban);
        evt.Currency.ShouldBe(Usd);
    }

    [Fact]
    public void Open_should_fail_when_owner_is_default()
    {
        Result<Account> result = Account.Open(default, Iban, Usd);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.OwnerRequired);
    }

    [Fact]
    public void Open_should_fail_when_number_is_null()
    {
        Result<Account> result = Account.Open(UserId.New(), null!, Usd);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AccountNumberRequired);
    }

    [Fact]
    public void Open_should_fail_when_currency_is_null()
    {
        Result<Account> result = Account.Open(UserId.New(), Iban, null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.CurrencyRequired);
    }

    [Fact]
    public void Credit_should_increase_balance_and_raise_event()
    {
        Account account = OpenActive();
        account.ClearDomainEvents();

        Result result = account.Credit(Money(100m));

        result.IsSuccess.ShouldBeTrue();
        account.Balance.Amount.ShouldBe(100m);
        var evt = account.DomainEvents.Single().ShouldBeOfType<AccountCreditedDomainEvent>();
        evt.AccountId.ShouldBe(account.Id);
        evt.Amount.Amount.ShouldBe(100m);
        evt.NewBalance.Amount.ShouldBe(100m);
    }

    [Fact]
    public void Credit_should_fail_when_amount_is_null()
    {
        Account account = OpenActive();

        Result result = account.Credit(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AmountRequired);
    }

    [Fact]
    public void Credit_should_fail_on_currency_mismatch()
    {
        Account account = OpenActive();

        Result result = account.Credit(Money(10m, Eur));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.CurrencyMismatch);
    }

    [Fact]
    public void Credit_should_fail_when_amount_is_zero_or_negative()
    {
        Account account = OpenActive();

        account.Credit(Money(0m)).Error.ShouldBe(AccountErrors.AmountMustBePositive);
        account.Credit(Money(-5m)).Error.ShouldBe(AccountErrors.AmountMustBePositive);
    }

    [Fact]
    public void Credit_should_fail_when_account_is_frozen()
    {
        Account account = OpenActive();
        account.Freeze();

        Result result = account.Credit(Money(10m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.NotActive);
    }

    [Fact]
    public void Credit_should_fail_when_account_is_closed()
    {
        Account account = OpenActive();
        account.Close();

        Result result = account.Credit(Money(10m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.NotActive);
    }

    [Fact]
    public void Debit_should_decrease_balance_and_raise_event()
    {
        Account account = OpenActive();
        account.Credit(Money(100m));
        account.ClearDomainEvents();

        Result result = account.Debit(Money(30m));

        result.IsSuccess.ShouldBeTrue();
        account.Balance.Amount.ShouldBe(70m);
        var evt = account.DomainEvents.Single().ShouldBeOfType<AccountDebitedDomainEvent>();
        evt.AccountId.ShouldBe(account.Id);
        evt.Amount.Amount.ShouldBe(30m);
        evt.NewBalance.Amount.ShouldBe(70m);
    }

    [Fact]
    public void Debit_should_fail_when_amount_is_null()
    {
        Account account = OpenActive();

        Result result = account.Debit(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AmountRequired);
    }

    [Fact]
    public void Debit_should_fail_on_currency_mismatch()
    {
        Account account = OpenActive();
        account.Credit(Money(100m));

        Result result = account.Debit(Money(10m, Eur));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.CurrencyMismatch);
    }

    [Fact]
    public void Debit_should_fail_when_amount_is_zero_or_negative()
    {
        Account account = OpenActive();
        account.Credit(Money(100m));

        account.Debit(Money(0m)).Error.ShouldBe(AccountErrors.AmountMustBePositive);
        account.Debit(Money(-5m)).Error.ShouldBe(AccountErrors.AmountMustBePositive);
    }

    [Fact]
    public void Debit_should_fail_when_balance_is_insufficient()
    {
        Account account = OpenActive();
        account.Credit(Money(10m));

        Result result = account.Debit(Money(20m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.InsufficientFunds);
        account.Balance.Amount.ShouldBe(10m);
    }

    [Fact]
    public void Debit_should_fail_when_account_is_frozen()
    {
        Account account = OpenActive();
        account.Credit(Money(100m));
        account.Freeze();

        Result result = account.Debit(Money(10m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.NotActive);
    }

    [Fact]
    public void Debit_should_fail_when_account_is_closed()
    {
        Account account = OpenActive();
        account.Credit(Money(100m));
        account.Close();

        Result result = account.Debit(Money(10m));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.NotActive);
    }

    [Fact]
    public void Freeze_should_transition_Active_to_Frozen()
    {
        Account account = OpenActive();

        Result result = account.Freeze();

        result.IsSuccess.ShouldBeTrue();
        account.Status.ShouldBe(AccountStatus.Frozen);
    }

    [Fact]
    public void Freeze_should_fail_when_already_frozen()
    {
        Account account = OpenActive();
        account.Freeze();

        Result result = account.Freeze();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AlreadyFrozen);
    }

    [Fact]
    public void Freeze_should_fail_when_closed()
    {
        Account account = OpenActive();
        account.Close();

        Result result = account.Freeze();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AlreadyClosed);
    }

    [Fact]
    public void Unfreeze_should_transition_Frozen_to_Active()
    {
        Account account = OpenActive();
        account.Freeze();

        Result result = account.Unfreeze();

        result.IsSuccess.ShouldBeTrue();
        account.Status.ShouldBe(AccountStatus.Active);
    }

    [Fact]
    public void Unfreeze_should_fail_when_not_frozen()
    {
        Account account = OpenActive();

        Result result = account.Unfreeze();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.NotFrozen);
    }

    [Fact]
    public void Unfreeze_should_fail_when_closed()
    {
        Account account = OpenActive();
        account.Close();

        Result result = account.Unfreeze();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AlreadyClosed);
    }

    [Fact]
    public void Close_should_transition_to_Closed_and_raise_event()
    {
        Account account = OpenActive();
        account.ClearDomainEvents();
        DateTime before = DateTime.UtcNow;

        Result result = account.Close();

        result.IsSuccess.ShouldBeTrue();
        account.Status.ShouldBe(AccountStatus.Closed);
        account.ClosedAt.ShouldNotBeNull();
        account.ClosedAt!.Value.ShouldBeGreaterThanOrEqualTo(before);
        var evt = account.DomainEvents.Single().ShouldBeOfType<AccountClosedDomainEvent>();
        evt.AccountId.ShouldBe(account.Id);
    }

    [Fact]
    public void Close_should_be_allowed_from_Frozen()
    {
        Account account = OpenActive();
        account.Freeze();
        account.ClearDomainEvents();

        Result result = account.Close();

        result.IsSuccess.ShouldBeTrue();
        account.Status.ShouldBe(AccountStatus.Closed);
        account.DomainEvents.Single().ShouldBeOfType<AccountClosedDomainEvent>();
    }

    [Fact]
    public void Close_should_fail_when_already_closed()
    {
        Account account = OpenActive();
        account.Close();

        Result result = account.Close();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountErrors.AlreadyClosed);
    }
}
