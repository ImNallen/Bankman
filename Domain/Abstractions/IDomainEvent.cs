namespace Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
