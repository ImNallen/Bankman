using Domain.Abstractions;
using Shouldly;

namespace Domain.UnitTests.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_should_create_successful_result()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Failure_should_create_failed_result()
    {
        var error = Error.Validation("Test.Error", "Something went wrong.");

        var result = Result.Failure(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Generic_failure_with_no_error_should_throw()
    {
        Should.Throw<InvalidOperationException>((Action)(() => Result.Failure<string>(Error.None)));
    }

    [Fact]
    public void Generic_success_should_contain_value()
    {
        var result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Generic_failure_should_throw_on_value_access()
    {
        var result = Result.Failure<int>(Error.Validation("Test", "Fail"));

        Should.Throw<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Implicit_conversion_from_value_should_create_success()
    {
        Result<string> result = "hello";

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("hello");
    }

    [Fact]
    public void Implicit_conversion_from_null_should_create_failure()
    {
        Result<string> result = (string?)null;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Error.NullValue);
    }
}
