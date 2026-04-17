using Domain.Users;
using Shouldly;

namespace Domain.UnitTests.Users;

public class UserIdTests
{
    [Fact]
    public void New_should_create_id_with_non_empty_guid()
    {
        UserId id = UserId.New();

        id.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void New_should_produce_unique_ids()
    {
        UserId a = UserId.New();
        UserId b = UserId.New();

        a.ShouldNotBe(b);
    }

    [Fact]
    public void From_should_wrap_provided_guid()
    {
        var guid = Guid.NewGuid();

        UserId id = UserId.From(guid);

        id.Value.ShouldBe(guid);
    }

    [Fact]
    public void Ids_with_same_value_should_be_equal()
    {
        var guid = Guid.NewGuid();
        UserId a = UserId.From(guid);
        UserId b = UserId.From(guid);

        a.ShouldBe(b);
        (a == b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void Ids_with_different_values_should_not_be_equal()
    {
        UserId a = UserId.From(Guid.NewGuid());
        UserId b = UserId.From(Guid.NewGuid());

        a.ShouldNotBe(b);
        (a != b).ShouldBeTrue();
    }
}
