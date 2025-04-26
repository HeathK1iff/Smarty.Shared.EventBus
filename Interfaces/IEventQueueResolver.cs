namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventQueueResolver
{
    bool TryGetQueue(Type eventType, out EventQueue? eventQueue);
}
