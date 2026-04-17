using Domain.Abstractions;
using Shouldly;

namespace Domain.UnitTests.Abstractions;

public class GuardTests
{
    [Fact]
    public void AgainstNullOrWhiteSpace_should_return_value_when_valid()
    {
        string result = Guard.AgainstNullOrWhiteSpace("hello");

        result.ShouldBe("hello");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AgainstNullOrWhiteSpace_should_throw_when_invalid(string? value)
    {
        Should.Throw<ArgumentException>(() => Guard.AgainstNullOrWhiteSpace(value));
    }

    [Fact]
    public void AgainstNull_should_return_value_when_not_null()
    {
        object obj = new object();

        object result = Guard.AgainstNull(obj);

        result.ShouldBeSameAs(obj);
    }

    [Fact]
    public void AgainstNull_should_throw_when_null()
    {
        Should.Throw<ArgumentNullException>(() => Guard.AgainstNull<object>(null));
    }

    [Fact]
    public void AgainstDefault_should_return_value_when_not_default()
    {
        var id = Guid.NewGuid();

        Guid result = Guard.AgainstDefault(id);

        result.ShouldBe(id);
    }

    [Fact]
    public void AgainstDefault_should_throw_when_default()
    {
        Should.Throw<ArgumentException>(() => Guard.AgainstDefault(Guid.Empty));
    }

    [Fact]
    public void AgainstNegative_should_return_value_when_zero()
    {
        decimal result = Guard.AgainstNegative(0m);

        result.ShouldBe(0m);
    }

    [Fact]
    public void AgainstNegative_should_return_value_when_positive()
    {
        decimal result = Guard.AgainstNegative(100m);

        result.ShouldBe(100m);
    }

    [Fact]
    public void AgainstNegative_should_throw_when_negative()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Guard.AgainstNegative(-1m));
    }

    [Fact]
    public void AgainstNegativeOrZero_should_return_value_when_positive()
    {
        decimal result = Guard.AgainstNegativeOrZero(50m);

        result.ShouldBe(50m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgainstNegativeOrZero_should_throw_when_zero_or_negative(int value)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Guard.AgainstNegativeOrZero((decimal)value));
    }
}
