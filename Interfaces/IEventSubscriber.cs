namespace Smarty.Shared.EventBus.Abstractions.Interfaces;

public interface IEventSubscriber
{
    Task Subscribe(Type eventType, Type eventHandlerType);
}
