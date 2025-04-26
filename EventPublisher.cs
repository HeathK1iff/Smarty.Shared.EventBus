using RabbitMQ.Client;
using Smarty.Shared.EventBus.Abstractions.Events;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;
using Smarty.Shared.EventBus.Options;

namespace Smarty.Shared.EventBus;

public sealed partial class EventBusChannelFactory
{
    public sealed class EventPublisher : IEventPublisher
    {
        readonly EventBusOptions _eventBusOptions;
        readonly IEventQueueResolver _eventQueueResolver;
        IChannel _channel;

        public EventPublisher(IChannel channel, 
            EventBusOptions eventBusOptions,
            IEventQueueResolver eventTypeResolver)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _eventBusOptions = eventBusOptions ?? throw new ArgumentNullException(nameof(eventBusOptions));
            _eventQueueResolver = eventTypeResolver ?? throw new ArgumentNullException(nameof(eventTypeResolver));
        }

        public async Task PublishAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (!await _eventQueueResolver.TryGetTopic(@event.GetType(), out var queue))
            {
                throw new Exception();
            }

            ArgumentNullException.ThrowIfNull(queue);
            
            var content = await queue.Serializator.SerializeAsync(@event);
            

            await _channel.BasicPublishAsync(_eventBusOptions.ExchangeName, @queue.QueueName, content, cancellationToken);
        }
    }

}
