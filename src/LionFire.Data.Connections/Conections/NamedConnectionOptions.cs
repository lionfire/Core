using System;
using System.Collections.Generic;
using LionFire.ExtensionMethods;

namespace LionFire.Data.Connections;

public class ConnectionOptions
{
    public static readonly string DefaultKey = "(default)";
}

public class NamedConnectionOptions<TConnectionOptions>
    //: Dictionary<string, TConnectionOptions>
    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
{

    public Dictionary<string, TConnectionOptions> Connections { get; set; } = new Dictionary<string, TConnectionOptions>();
    public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    //public Dictionary<string, string> Connections { get; set; } = new Dictionary<string, string>();

    public TConnectionOptions GetOptionsOrDefault(string connectionName)
    {

        var result = Connections?.TryGetValue(connectionName);
        if (result != null) return result;

        if(typeof(IHasConnectionStringRW).IsAssignableFrom(typeof(TConnectionOptions)) && ConnectionStrings.ContainsKey(connectionName))
        {
            result = Activator.CreateInstance<TConnectionOptions>();
            ((IHasConnectionStringRW)result).ConnectionString = ConnectionStrings[connectionName];
            return result;
        }
        //?? Connections?.TryGetValue(ConnectionOptions.DefaultKey);
        return null;
    }
}

// OLD - works but is a massively deep appsettings.json
//public class ConnectionManagerOptions<TConnectionOptions> 
//    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
//{

//    public Dictionary<string, TConnectionOptions> Connections { get; set; }
//}
