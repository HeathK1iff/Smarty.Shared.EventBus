using RabbitMQ.Client;

namespace Smarty.Shared.EventBus.Interfaces;

public interface IEventBusChannelFactory
{
    Task<IChannel> CreateAndDeclareExchangeAsync(CancellationToken cancellationToken);
}
