using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventPublisher
{
    public Task PublishAsync(EventBase @event, CancellationToken cancellationToken);
}
