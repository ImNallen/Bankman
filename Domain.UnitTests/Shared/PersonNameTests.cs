using Domain.Abstractions;
using Domain.Shared;
using Shouldly;

namespace Domain.UnitTests.Shared;

public class PersonNameTests
{
    [Fact]
    public void Create_should_succeed_and_trim_both_parts()
    {
        Result<PersonName> result = PersonName.Create("  Ada  ", "  Lovelace  ");

        result.IsSuccess.ShouldBeTrue();
        result.Value.FirstName.ShouldBe("Ada");
        result.Value.LastName.ShouldBe("Lovelace");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_when_first_name_empty(string? first)
    {
        Result<PersonName> result = PersonName.Create(first, "Lovelace");

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PersonName.FirstNameEmpty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_should_fail_when_last_name_empty(string? last)
    {
        Result<PersonName> result = PersonName.Create("Ada", last);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PersonName.LastNameEmpty);
    }
}
