using RabbitMQ.Client;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;
using Smarty.Shared.EventBus.Options;
using Smarty.Shared.EventBus.Validation;

namespace Smarty.Shared.EventBus;

public sealed partial class EventBusChannelFactory : IEventBusChannelFactory
{
    IConnection? _connection;
    readonly EventBusOptions _options;
    readonly IEventQueueResolver _eventTypeResolver;

    public EventBusChannelFactory(EventBusOptions options, IEventQueueResolver eventTypeResolver)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _eventTypeResolver = eventTypeResolver ?? throw new ArgumentNullException(nameof(eventTypeResolver));
    }

    public async Task<IEventSubscriber> CreateSubscriberAsync(CancellationToken cancellationToken)
    {
        var connection = await GetOrCreateChannelAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(_options.ExchangeName, type: ExchangeType.Direct);

        return new EventSubscriber(channel, _eventTypeResolver, cancellationToken);
    }

    public async Task<IEventPublisher> CreatePublisherAsync(CancellationToken cancellationToken)
    {
        var connection = await GetOrCreateChannelAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(_options.ExchangeName, type: ExchangeType.Direct);

        return new EventPublisher(channel, _options, _eventTypeResolver);
    }

    private async Task<IConnection> GetOrCreateChannelAsync(CancellationToken cancellationToken)
    {
         if (_connection is null)
        {
            _options.ThrowIfNotValid();

            var factory = new ConnectionFactory
            {
                UserName = _options.UserName!,
                Password = _options.Password!,
                VirtualHost = "/",
                HostName = _options.HostName!,
                ClientProvidedName = _options.ClientProvidedName
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
        }

        return _connection;
    }
}
