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
        readonly EventBusConnectionString _eventBusOptions;
        readonly IEventQueueResolver _eventQueueResolver;
        readonly IChannel _channel;

        public EventPublisher(IChannel channel, 
            EventBusConnectionString eventBusOptions,
            IEventQueueResolver eventTypeResolver)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _eventBusOptions = eventBusOptions ?? throw new ArgumentNullException(nameof(eventBusOptions));
            _eventQueueResolver = eventTypeResolver ?? throw new ArgumentNullException(nameof(eventTypeResolver));
        }

        public void Publish(EventBase @event)
        {
            PublishAsync(@event, CancellationToken.None).GetAwaiter();
        }

        public async Task PublishAsync(EventBase @event, CancellationToken cancellationToken)
        {
            if (!_eventQueueResolver.TryGetQueue(@event.GetType(), out var queue))
            {
                throw new Exception();
            }

            ArgumentNullException.ThrowIfNull(queue);
            
            var content = queue.Serializator.Serialize(@event);

            await _channel.BasicPublishAsync(_eventBusOptions.ExchangeName, @queue.QueueName, content, cancellationToken);
        }
    }

}
