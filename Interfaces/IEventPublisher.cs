using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventPublisher : IDisposable
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T: EventBase;
    void Publish<T>(T @event) where T: EventBase;
}
