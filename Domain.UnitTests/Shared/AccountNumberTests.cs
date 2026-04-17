using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class AccountNumberTests
{
    [Theory]
    [InlineData("GB82 WEST 1234 5698 7654 32", "GB82WEST12345698765432")]
    [InlineData("de89 3704 0044 0532 0130 00", "DE89370400440532013000")]
    [InlineData("FR1420041010050500013M02606", "FR1420041010050500013M02606")]
    [InlineData("NL91ABNA0417164300", "NL91ABNA0417164300")]
    [InlineData("BE68539007547034", "BE68539007547034")]
    public void Create_should_succeed_for_valid_iban(string input, string expected)
    {
        Result<AccountNumber> result = AccountNumber.Create(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_for_empty(string? input)
    {
        Result<AccountNumber> result = AccountNumber.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountNumber.Empty);
    }

    [Theory]
    // Bad checksum
    [InlineData("GB00WEST12345698765432")]
    [InlineData("GB82WEST12345698765431")]
    [InlineData("DE00370400440532013000")]
    // Wrong length
    [InlineData("GB82WEST1234569876543")]
    [InlineData("GB82WEST123456987654321")]
    [InlineData("DE8937040044053201300")]
    // Invalid structure
    [InlineData("ZZ82WEST12345698765432")]
    [InlineData("1282WEST12345698765432")]
    [InlineData("GBAAWEST12345698765432")]
    [InlineData("GB82WEST1234569876543!")]
    public void Create_should_fail_for_invalid_iban(string input)
    {
        Result<AccountNumber> result = AccountNumber.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AccountNumber.Invalid);
    }
}
