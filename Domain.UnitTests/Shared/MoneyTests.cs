using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class MoneyTests
{
    private static Currency Usd => Currency.Create("USD").Value;
    private static Currency Eur => Currency.Create("EUR").Value;

    [Fact]
    public void Create_should_succeed_for_valid_amount_and_currency()
    {
        Result<Money> result = Money.Create(42.50m, Usd);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(42.50m);
        result.Value.Currency.ShouldBe(Usd);
    }

    [Fact]
    public void Create_should_fail_when_currency_is_null()
    {
        Result<Money> result = Money.Create(10m, null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Money.CurrencyRequired);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_should_fail_when_amount_is_negative(decimal amount)
    {
        Result<Money> result = Money.Create(amount, Usd);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Money.NegativeAmount);
    }

    [Fact]
    public void Create_should_succeed_when_amount_is_zero()
    {
        Result<Money> result = Money.Create(0m, Usd);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(0m);
        result.Value.Currency.ShouldBe(Usd);
    }

    [Fact]
    public void Zero_should_create_zero_amount_money_in_currency()
    {
        Money zero = Money.Zero(Usd);

        zero.Amount.ShouldBe(0m);
        zero.Currency.ShouldBe(Usd);
    }

    [Fact]
    public void Add_should_sum_amounts_with_same_currency()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(5m, Usd).Value;

        Result<Money> result = a.Add(b);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(15m);
        result.Value.Currency.ShouldBe(Usd);
    }

    [Fact]
    public void Subtract_should_difference_amounts_with_same_currency()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(3m, Usd).Value;

        Result<Money> result = a.Subtract(b);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(7m);
    }

    [Fact]
    public void Add_should_fail_on_currency_mismatch()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(5m, Eur).Value;

        Result<Money> result = a.Add(b);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Money.CurrencyMismatch);
    }

    [Fact]
    public void Subtract_should_fail_on_currency_mismatch()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(5m, Eur).Value;

        Result<Money> result = a.Subtract(b);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Money.CurrencyMismatch);
    }

    [Fact]
    public void Monies_with_same_amount_and_currency_should_be_equal()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(10m, Usd).Value;

        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void Monies_with_different_amounts_should_not_be_equal()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(11m, Usd).Value;

        a.ShouldNotBe(b);
    }

    [Fact]
    public void Monies_with_different_currencies_should_not_be_equal()
    {
        Money a = Money.Create(10m, Usd).Value;
        Money b = Money.Create(10m, Eur).Value;

        a.ShouldNotBe(b);
    }
}
