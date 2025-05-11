using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Smarty.Shared.EventBus.Options;

public class EventBusConnectionString 
{
    readonly Dictionary<string, string> _dict = new();
    public string? UserName 
    { 
        get
        {
            return _dict.TryGetValue("User", out var val) ? val : string.Empty;
        } 
        set
        {
            if (value is null)
            {
                return;
            }

            _dict["User"] = value; 
        }
    }
    public string? Password 
    { 
        get
        {
            return _dict.TryGetValue("Password", out var val) ? val : string.Empty;
        } 
        set
        {
            if (value is null)
            {
                return;
            }

            _dict["Password"] = value; 
        }
    }

    public string? HostName 
    { 
        get
        {
            return _dict.TryGetValue("Host", out var val) ? val : string.Empty;
        } 
        set
        {
            if (value is null)
            {
                return;
            }

            _dict["Host"] = value; 
        }
    }
    
    public string ClientProvidedName 
    { 
        get
        {
            return _dict.TryGetValue("ClientProvidedName", out var val) ? val : string.Empty;
        } 
        set
        {
            if (value is null)
            {
                return;
            }

            _dict["ClientProvidedName"] = value; 
        }
    }

    public string ExchangeName 
    { 
        get
        {
            return _dict.TryGetValue("Exchange", out var val) ? val : string.Empty;
        } 
        set
        {
            if (value is null)
            {
                return;
            }

            _dict["Exchange"] = value; 
        } 
    }


    public int Port 
    { 
        get
        {
            return _dict.TryGetValue("Port", out var val) ? (int.TryParse(val, out var val2) ? val2 : 0) : 5672;
        } 
        set
        {
            _dict["Port"] = Convert.ToString(value); 
        } 
    }

    public EventBusConnectionString()
    {
    }

    private EventBusConnectionString(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        foreach (var key in keyValuePairs)
        {
            _dict.Add(key.Key, key.Value);
        }
    }

    public static EventBusConnectionString Parce(string input)
    {
        var dict = new Dictionary<string, string>();
        foreach (var keyValuePair in  input.Split(";"))
        {
            var matches = Regex.Match(keyValuePair, @"^(\w+)=(.+)$");

            if (matches.Success)
            {
                dict.TryAdd(matches.Groups[1].Value, matches.Groups[2].Value);
            }
        }

        return new EventBusConnectionString(dict);
    }

    public override string ToString()
    {
        return $"Host={HostName};User={UserName};Port={Port};Password={Password};ClientProvidedName={ClientProvidedName};Exchange={ExchangeName}";
    }
}


