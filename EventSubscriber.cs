using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;

namespace Smarty.Shared.EventBus;

public sealed partial class EventBusChannelFactory
{
    private sealed class EventSubscriber : IEventSubscriber
    {
        readonly CancellationToken _cancellationToken;
        readonly IEventQueueResolver _eventQueueResolver;
        readonly IChannel _channel;

        public EventSubscriber(IChannel channel, 
            IEventQueueResolver eventQueueResolver,
            CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _eventQueueResolver = eventQueueResolver ?? throw new ArgumentNullException(nameof(eventQueueResolver));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public async Task Subscribe(Type eventType, IEventHandler eventHandler)
        {
            if (!_eventQueueResolver.TryGetQueue(eventType, out var queue))
            {
                throw new Exception();
            }

            ArgumentNullException.ThrowIfNull(queue);

            await _channel.QueueDeclareAsync(queue.QueueName, 
                queue.Options?.Durable ?? false, 
                cancellationToken: _cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var @event = await queue.Serializator.DeserializeAsync(ea.Body.ToArray(), eventType, _cancellationToken);

                if (@event is null)
                {
                    return;
                }

                await eventHandler.ReceivedAsync(@event, _cancellationToken);
            };
        }
    }

}
