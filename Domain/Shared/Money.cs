using Domain.Abstractions;

namespace Domain.Shared;

public sealed record Money : ValueObject
{
    public static readonly Error CurrencyRequired = Error.Validation(
        "Money.CurrencyRequired",
        "Currency is required.");

    public static readonly Error CurrencyMismatch = Error.Validation(
        "Money.CurrencyMismatch",
        "Monetary amounts must share the same currency.");

    public static readonly Error NegativeAmount = Error.Validation(
        "Money.NegativeAmount",
        "A monetary amount cannot be negative.");

    public decimal Amount { get; }

    public Currency Currency { get; }

    private Money()
    {
    }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (currency is null)
        {
            return Result.Failure<Money>(CurrencyRequired);
        }

        if (amount < 0m)
        {
            return Result.Failure<Money>(NegativeAmount);
        }

        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        return new Money(0m, currency);
    }

    public Result<Money> Add(Money other)
    {
        if (other is null)
        {
            return Result.Failure<Money>(Error.NullValue);
        }

        if (Currency != other.Currency)
        {
            return Result.Failure<Money>(CurrencyMismatch);
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (other is null)
        {
            return Result.Failure<Money>(Error.NullValue);
        }

        if (Currency != other.Currency)
        {
            return Result.Failure<Money>(CurrencyMismatch);
        }

        decimal result = Amount - other.Amount;

        if (result < 0m)
        {
            return Result.Failure<Money>(NegativeAmount);
        }

        return new Money(result, Currency);
    }

    public override string ToString() => $"{Amount} {Currency.Code}";
}
