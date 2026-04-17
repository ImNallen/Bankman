using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class PasswordHashTests
{
    [Fact]
    public void Create_should_succeed_for_non_empty_value()
    {
        Result<PasswordHash> result = PasswordHash.Create("$argon2id$hash");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("$argon2id$hash");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_for_empty(string? input)
    {
        Result<PasswordHash> result = PasswordHash.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PasswordHash.Empty);
    }
}
