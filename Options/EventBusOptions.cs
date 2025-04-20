namespace Smarty.Shared.EventBus.Options;

public class EventBusOptions
{
    public const string SectionName = "EventBus";

    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? HostName { get; init; }
    public string ClientProvidedName { get; init; } =  "smarty";
    public string ExchangeName { get; init; } =  "smarty";
}
