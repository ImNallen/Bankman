using Domain.Abstractions;
using Shouldly;

namespace Domain.UnitTests.Abstractions;

public class EntityTests
{
    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id)
        {
        }
    }

    private sealed class OtherEntity : Entity<Guid>
    {
        public OtherEntity(Guid id) : base(id)
        {
        }
    }

    [Fact]
    public void Entities_with_same_id_should_be_equal()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.Equals(entity2).ShouldBeTrue();
    }

    [Fact]
    public void Entities_with_different_ids_should_not_be_equal()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        entity1.Equals(entity2).ShouldBeFalse();
    }

    [Fact]
    public void Entity_should_equal_itself()
    {
        var entity = new TestEntity(Guid.NewGuid());
        TestEntity entity2 = entity;

        entity.Equals(entity2).ShouldBeTrue();
    }

    [Fact]
    public void Entities_with_same_id_should_have_same_hash_code()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.GetHashCode().ShouldBe(entity2.GetHashCode());
    }

    [Fact]
    public void Different_entity_types_with_same_id_should_be_equal()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new OtherEntity(id);

        entity1.Equals(entity2).ShouldBeTrue();
    }
}
