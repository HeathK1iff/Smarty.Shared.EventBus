namespace Smarty.Shared.EventBus;

public sealed class QueueOptions
{
    public bool Durable { get; init; } = false;
    public bool Exclusive { get; init; } = true;
}
