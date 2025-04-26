namespace Smarty.Shared.EventBus.Abstractions.Events;

public abstract class EventBase
{
    public Guid Id { get; init; }
    public DateTime Created { get; init; }
    public string? Sender { get; init; }
}
