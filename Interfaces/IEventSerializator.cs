using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventSerializator
{
    Task<EventBase> DeserializeAsync(byte[] content, CancellationToken cancellationToken);

    Task<byte[]> SerializeAsync(EventBase content);
}
