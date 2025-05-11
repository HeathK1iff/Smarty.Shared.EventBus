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
        private bool disposedValue;

        public EventPublisher(IChannel channel, 
            EventBusConnectionString eventBusOptions,
            IEventQueueResolver eventTypeResolver)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _eventBusOptions = eventBusOptions ?? throw new ArgumentNullException(nameof(eventBusOptions));
            _eventQueueResolver = eventTypeResolver ?? throw new ArgumentNullException(nameof(eventTypeResolver));
        }

        public void Publish<T>(T @event) where T: EventBase
        {
            PublishAsync(@event, CancellationToken.None).GetAwaiter();
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T: EventBase
        {
            if (!_eventQueueResolver.TryGetQueue(@event.GetType(), out var queue))
            {
                throw new Exception();
            }

            ArgumentNullException.ThrowIfNull(queue);
            
            
            await _channel.QueueDeclareAsync(queue: queue.QueueName, 
                exclusive: queue.Options?.Exclusive ?? false,
                durable: queue.Options?.Durable ?? false, 
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(queue.QueueName, _eventBusOptions.ExchangeName, queue.QueueName);

            var content = queue.Serializator.Serialize(@event);

            await _channel.BasicPublishAsync(_eventBusOptions.ExchangeName, routingKey: @queue.QueueName, content, cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   _channel.CloseAsync().GetAwaiter();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
