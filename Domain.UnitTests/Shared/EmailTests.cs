using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class EmailTests
{
    [Fact]
    public void Create_should_succeed_for_valid_address()
    {
        Result<Email> result = Email.Create("user@example.com");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("user@example.com");
    }

    [Fact]
    public void Create_should_trim_and_lower_case()
    {
        Result<Email> result = Email.Create("  User@Example.COM  ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("user@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_for_empty(string? input)
    {
        Result<Email> result = Email.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Email.Empty);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@@example.com")]
    [InlineData("user@ example.com")]
    public void Create_should_fail_for_invalid_format(string input)
    {
        Result<Email> result = Email.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Email.Invalid);
    }
}
