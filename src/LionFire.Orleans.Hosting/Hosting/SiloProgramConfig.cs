#nullable enable
using LionFire.Configuration;
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using System.Net;
using static LionFire.Net.PortConfiguration;
using LionFire.ExtensionMethods.Configuration;

namespace LionFire.Hosting;

// TODO: Split into HostSiloProgramOptions and SiloApplicationOptions since some settings (ports and interfaces) are used while building Host and Configure is too late for it.
// TODO: Bind to Configuration[ConfigLocation], populate from appsettings.json instead of hardcoded C#?
// TODO: Split into subclasses
//  - ConsulServiceConfig
//  - RelativePortsConfig - not hardcoded.  Add some place to look up available ports.
public class SiloProgramConfig : HasPortsConfigBase, IHasConfigLocation
{
    public static string DefaultConfigLocation => "Silo";
    public string ConfigLocation => DefaultConfigLocation;

    //protected int? BasePort { get; set; }

    #region Construction

    public SiloProgramConfig(IConfiguration configuration) : base(configuration)
    {
        //var portsConfig = new PortsConfig(configuration);
        //BasePort = portsConfig.EffectiveBasePort;
        this.Bind(configuration);
    }

    #endregion

    #region Interface and Port

    #region Orleans Silo URLs

    public string SiloInterface { get; set; } = "127.0.0.1"; // TODO - configure

    #region Orleans Silo Port

    public int? SiloPort
    {
        get => GetPortOrOffset(siloPort, BasePort, SiloPortOffset);
        set => siloPort = value;
    }
    public int? siloPort = null;
    public int SiloPortOffset { get; set; } = 2;

    #endregion

    #region Orleans Gateway Port

    public int? GatewayPort
    {
        get => GetPortOrOffset(gatewayPort, BasePort, GatewayPortOffset);
        set => gatewayPort = value;
    }
    public int? gatewayPort = null;
    public int GatewayPortOffset { get; set; } = 3;

    #endregion

    #endregion

    #region Silo-specific Health Check

    public bool SiloHealthCheckEnabled { get; set; } = true;

    /// <summary>
    /// Respects SiloHealthCheckScheme, if true
    /// </summary>
    public bool SiloHealthCheckOnPrimaryWebHost { get; set; } = true; // OLD - eliminate this, force to true.  
    
    public int? SiloHealthCheckPort
    {
        get => GetPortOrOffset(siloHealthCheckPort, BasePort, SiloHealthCheckPortOffset);
        set => siloHealthCheckPort = value;
    }
    public int? siloHealthCheckPort = null;
    public int SiloHealthCheckPortOffset { get; set; } = 5;

    public string SiloHealthCheckInterface { get; set; } = "localhost";
    public string SiloHealthCheckScheme { get; set; } = "http"; // Limitation: can currently only have one of http and https

    #endregion

    #endregion

    #region Orleans Dashboard

    public bool DashboardEnabled { get; set; } = true;
    public string DashboardInterface { get; set; } = "127.0.0.1";

    public int? DashboardPort
    {
        get => GetPortOrOffset(dashboardPort, BasePort, DashboardPortOffset);
        set => dashboardPort = value;
    }
    public int? dashboardPort = null;
    public int DashboardPortOffset { get; set; } = 4;

    #endregion

    public IPEndPoint? LocalhostPrimaryClusterEndpoint { get; set; }


}
