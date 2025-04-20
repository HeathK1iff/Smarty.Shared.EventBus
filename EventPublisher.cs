
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Smarty.Shared.EventBus.Abstractions.Events;
using Smarty.Shared.EventBus.Abstractions.Interfaces;
using Smarty.Shared.EventBus.Interfaces;

namespace Smarty.Shared.EventBus;

public class EventPublisher : IEventPublisher
{
    readonly IEventBusChannelFactory _eventBusConnection;
    IChannel? _channel;

    public EventPublisher(IEventBusChannelFactory eventBusConnection)
    {
        _eventBusConnection = eventBusConnection ?? throw new ArgumentNullException(nameof(eventBusConnection));
    }

    public async Task PublishAsync(EventBase @event, CancellationToken cancellationToken)
    {
        if (_channel == null)
        {
            _channel = await _eventBusConnection.CreateAndDeclareExchangeAsync(cancellationToken);
        }

        var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        await _channel.BasicPublishAsync("aa", "aa", content);
    }
}
