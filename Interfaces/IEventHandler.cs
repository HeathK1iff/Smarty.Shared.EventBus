using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventHandler
{
    Task ReceivedAsync(EventBase @event, CancellationToken cancellationToken);
}
