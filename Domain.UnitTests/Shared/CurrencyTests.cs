using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class CurrencyTests
{
    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void Create_should_succeed_for_three_upper_case_letters(string code)
    {
        Result<Currency> result = Currency.Create(code);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Code.ShouldBe(code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_for_empty(string? code)
    {
        Result<Currency> result = Currency.Create(code);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Currency.Empty);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDX")]
    [InlineData("usd")]
    [InlineData("US1")]
    [InlineData("US$")]
    public void Create_should_fail_for_invalid_code(string code)
    {
        Result<Currency> result = Currency.Create(code);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Currency.Invalid);
    }
}
