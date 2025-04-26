namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventQueueResolver
{
    Task<bool> TryGetTopic(Type eventType, out EventQueue? topic);
}
