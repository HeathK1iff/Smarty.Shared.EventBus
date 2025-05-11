using RabbitMQ.Client;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;
using Smarty.Shared.EventBus.Options;

namespace Smarty.Shared.EventBus;

public sealed partial class EventBusChannelFactory : IEventBusChannelFactory
{
    IConnection? _connection;
    readonly EventBusConnectionString _options;
    readonly IEventQueueResolver _eventTypeResolver;
    readonly IServiceProvider _serviceProvider;

    public EventBusChannelFactory(string connectionString, IEventQueueResolver eventTypeResolver,
        IServiceProvider serviceProvider)
    {
        _options = EventBusConnectionString.Parce(connectionString);
        _eventTypeResolver = eventTypeResolver ?? throw new ArgumentNullException(nameof(eventTypeResolver));
        _serviceProvider  = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<IEventSubscriber> CreateSubscriberAsync(CancellationToken cancellationToken)
    {
        var connection = await GetOrCreateChannelAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(_options.ExchangeName, type: ExchangeType.Direct);

        return new EventSubscriber(channel, _eventTypeResolver, cancellationToken, _serviceProvider);
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
            var factory = new ConnectionFactory
            {
                UserName = _options.UserName!,
                Password = _options.Password!,
                Port = _options.Port,
                VirtualHost = "/",
                HostName = _options.HostName!,
                ClientProvidedName = _options.ClientProvidedName
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
        }

        return _connection;
    }

    public IEventPublisher CreatePublisher()
    {
        return CreatePublisherAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public IEventSubscriber CreateSubscriberAsync()
    {
        return CreateSubscriberAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
}
