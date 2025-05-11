using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(EventBase @event, CancellationToken cancellationToken);
    void Publish(EventBase @event);
}
