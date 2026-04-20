using Domain.Abstractions;
using MediatR;

namespace Infrastructure.Events;

internal sealed record DomainEventNotification<TEvent>(TEvent Event) : INotification
    where TEvent : IDomainEvent;
