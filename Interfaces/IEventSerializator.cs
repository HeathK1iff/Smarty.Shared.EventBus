using Smarty.Shared.EventBus.Abstractions.Events;

namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventSerializator
{
    Task<EventBase?> DeserializeAsync(byte[] content, Type type, CancellationToken cancellationToken);

    byte[] Serialize<T>(T content) where T: EventBase;
}
