namespace Smarty.Shared.EventBus.Abstractions.Events;

public abstract class EventBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public int Version { get; init; }
}
