using RabbitMQ.Client;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;
using Smarty.Shared.EventBus.Options;
using Smarty.Shared.EventBus.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Smarty.Shared.EventBus;

public sealed partial class EventBusChannelFactory : IEventBusChannelFactory
{
    IConnection? _connection;
    readonly EventBusOptions _options;
    readonly IEventQueueResolver _eventTypeResolver;
    readonly IServiceProvider _serviceProvider;

    public EventBusChannelFactory(IOptions<EventBusOptions> options, IEventQueueResolver eventTypeResolver,
        IServiceProvider serviceProvider)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
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
