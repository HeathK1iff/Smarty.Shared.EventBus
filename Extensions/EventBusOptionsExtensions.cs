using System.ComponentModel.DataAnnotations;
using Smarty.Shared.EventBus.Options;

namespace Smarty.Shared.EventBus.Validation;

public static class EventBusOptionsExtensions
{
    public static void ThrowIfNotValid(this EventBusOptions eventBusOptions)
    {
        if (string.IsNullOrWhiteSpace(eventBusOptions.HostName))
        {
            throw new ValidationException("Host name is empty");
        }
        
        // RuleFor(f => f.UserName).NotNull();
        // RuleFor(f => f.Password).NotNull();
        // RuleFor(f => f.HostName).NotNull();
    }

}
