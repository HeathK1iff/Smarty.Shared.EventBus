using FluentValidation;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Smarty.Shared.EventBus.Interfaces;
using Smarty.Shared.EventBus.Options;
using Smarty.Shared.EventBus.Validation;

namespace Smarty.Shared.EventBus;

public class EventBusChannelFactory : IEventBusChannelFactory
{
    IConnection? _connection;
    readonly EventBusOptions _options;

    public EventBusChannelFactory(IOptions<EventBusOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IChannel> CreateAndDeclareExchangeAsync(CancellationToken cancellationToken)
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

        var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(_options.ExchangeName, type: ExchangeType.Direct);

        return channel;
    }
}
