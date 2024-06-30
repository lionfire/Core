using DynamicData;
using DynamicData.Binding;
using LionFire.Threading;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace LionFire.Net.MultiConnection;

public class ApiConnection
{

    public ServerAddress Address { get; }

    public ApiConnection(ServerAddress address)
    {
        Address = address;
    }

    public bool IsConnected { get; }
    public bool IsConnecting { get; }
}

public class ServerAddress
{
    public Guid Key { get; set; }

    public int Weight { get; set; } = 100;

    public bool InProcess { get; set; }
    public string? Host { get; set; }
    public string? AddressIPv4 { get; set; }
    public string? AddressIPv6 { get; set; }

    // TODO: Service to determine pings
    // TODO: Service to query server's remaining capacity
    // TODO: Service to query server time until shutdown
}

//public class MagicOnionApiConnection : ApiConnection
//{
//    public MagicOnionApiConnection(ServerAddress address) : base(address)
//    {
//    }
//}

public interface IApiConnectionParameters
{
    /// <summary>
    /// Key for the API (e.g. "Universe.Valor.Beta")
    /// </summary>
    string Key { get; }

    IObservableCache<ServerAddress, Guid> Addresses { get; }
}

public class ApiConnectionOptions
{
    public bool AutoConnect { get; set; } = true;

    // ENH idea: to avoid interruption if servers go down or are upgraded.  Can also potentially help with latency and chaos engineering: send request to both servers, and make sure only one server commits the action
    public int DesiredConnectionCount { get; set; } = 2;
}

public class ApiConnectionStateMachine
{
    #region Parameters

    public IApiConnectionParameters Parameters { get; }

    #endregion

    #region Options

    public IOptionsMonitor<ApiConnectionOptions> OptionsMonitor { get; }
    public ApiConnectionOptions Options => OptionsMonitor.CurrentValue;

    #endregion

    #region Lifecycle

    public ApiConnectionStateMachine(IApiConnectionParameters parameters, IOptionsMonitor<ApiConnectionOptions> optionsMonitor)
    {
        Parameters = parameters;
        OptionsMonitor = optionsMonitor;

        //Parameters.Addresses.Connect()
        //    .Bind(out addresses)
        //    .Subscribe();

        if (Options.AutoConnect)
        {
            Connect().FireAndForget();
        }
    }

    #endregion

    #region State

    public IObservableCache<ServerAddress, Guid> Addresses => Parameters.Addresses;

    public ConcurrentDictionary<string, ApiConnection> Connections { get; } = new ConcurrentDictionary<string, ApiConnection>();

    public IDictionary<Guid, ApiConnection> ConnectedOrConnectingServers
        => Connections.Values.Where(c => c.IsConnected || c.IsConnecting).ToDictionary(c => c.Address.Key);

    public IEnumerable<ServerAddress> UnconnectedServers
        => Addresses.Items.Where(a => !ConnectedOrConnectingServers.ContainsKey(a.Key));

    public int AvailableServersCount
        => Addresses.Items.Select(a => a.Key).Concat(Connections.Values.Select(c => c.Address.Key)).Distinct().Count();

    #endregion

    private async ValueTask Connect()
    {
        List<ValueTask> tasks = new List<ValueTask>();

        while (ConnectedOrConnectingServers.Count() < Options.DesiredConnectionCount && UnconnectedServers.Any()
            //.Values.Count(c => c.IsConnected) < Math.Min(Addresses.Count, Options.DesiredConnectionCount)
            )
        {
            tasks.Add(ConnectOne());
        }

        foreach (var task in tasks)
        {
            await task.ConfigureAwait(false);
        }
    }
    private ValueTask ConnectOne()
    {
        //UnconnectedServers

        throw new NotImplementedException();
    }
}




