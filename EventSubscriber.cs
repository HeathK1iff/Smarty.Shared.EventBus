using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
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
        readonly IServiceProvider _serviceProvider;

        public EventSubscriber(IChannel channel, 
            IEventQueueResolver eventQueueResolver,
            CancellationToken cancellationToken,
            IServiceProvider serviceProvider)
        {
            _cancellationToken = cancellationToken;
            _eventQueueResolver = eventQueueResolver ?? throw new ArgumentNullException(nameof(eventQueueResolver));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _serviceProvider  = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Subscribe(Type eventType, Type eventHandlerType)
        {
            SubscribeAsync(eventType, eventHandlerType).GetAwaiter();
        }

        public async Task SubscribeAsync(Type eventType, Type eventHandlerType)
        {
            if (!_eventQueueResolver.TryGetQueue(eventType, out var queue))
            {
                throw new Exception();
            }

            ArgumentNullException.ThrowIfNull(queue);

            await _channel.QueueDeclareAsync(queue: queue.QueueName, 
                exclusive: queue.Options?.Exclusive ?? false,
                durable: queue.Options?.Durable ?? false, 
                cancellationToken: _cancellationToken);
            
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            { 
                var @event = await queue.Serializator.DeserializeAsync(ea.Body.ToArray(), eventType, _cancellationToken);

                if (@event is null)
                {
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var serviceHandler = scope.ServiceProvider.GetRequiredService(eventHandlerType) as IEventHandler;

                await serviceHandler!.ReceivedAsync(@event, _cancellationToken);
            };
        }
    }

}
