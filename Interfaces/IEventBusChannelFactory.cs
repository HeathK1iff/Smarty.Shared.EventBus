using Smarty.Shared.EventBus.Abstractions.Interfaces;

namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventBusChannelFactory
{
    Task<IEventSubscriber> CreateSubscriberAsync(CancellationToken cancellationToken);

    Task<IEventPublisher> CreatePublisherAsync(CancellationToken cancellationToken);

    IEventPublisher CreatePublisher();

    IEventSubscriber CreateSubscriberAsync();
}
