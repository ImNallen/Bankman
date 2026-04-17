using Domain.Abstractions;
using Shouldly;

namespace Domain.UnitTests.Abstractions;

public class AggregateRootTests
{
    private sealed record TestEvent(DateTime OccurredOn) : IDomainEvent;

    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id)
        {
        }

        public void DoSomething()
        {
            RaiseDomainEvent(new TestEvent(DateTime.UtcNow));
        }
    }

    [Fact]
    public void New_aggregate_should_have_no_domain_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RaiseDomainEvent_should_add_event_to_collection()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();

        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents.First().ShouldBeOfType<TestEvent>();
    }

    [Fact]
    public void RaiseDomainEvent_should_accumulate_multiple_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();
        aggregate.DoSomething();
        aggregate.DoSomething();

        aggregate.DomainEvents.Count.ShouldBe(3);
    }

    [Fact]
    public void ClearDomainEvents_should_remove_all_events()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething();
        aggregate.DoSomething();

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }
}
