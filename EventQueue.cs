using Smarty.Shared.EventBus.Interfaces;

namespace Smarty.Shared.EventBus;

public sealed class EventQueue
{
    public required string QueueName { get; init; }
    public required string RoutingKey { get; init; }
    public QueueOptions? Options { get; init; }
    public required IEventSerializator Serializator { get; init; }
}
