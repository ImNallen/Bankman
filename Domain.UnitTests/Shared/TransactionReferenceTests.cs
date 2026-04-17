using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class TransactionReferenceTests
{
    [Fact]
    public void Create_should_succeed_and_trim()
    {
        Result<TransactionReference> result = TransactionReference.Create("  Invoice 123  ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe("Invoice 123");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_for_empty(string? input)
    {
        Result<TransactionReference> result = TransactionReference.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionReference.Empty);
    }

    [Fact]
    public void Create_should_fail_when_too_long()
    {
        string input = new('x', TransactionReference.MaxLength + 1);

        Result<TransactionReference> result = TransactionReference.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TransactionReference.TooLong);
    }
}
