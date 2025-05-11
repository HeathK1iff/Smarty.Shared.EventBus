namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventSubscriber
{
    Task SubscribeAsync(Type eventType, Type eventHandlerType);

    void Subscribe(Type eventType, Type eventHandlerType);
}
