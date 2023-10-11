#nullable enable
using System.Net;
using System.Reflection;
using System.Text;

namespace LionFire.Hosting;

// TODO: Split into HostSiloProgramOptions and SiloApplicationOptions since some settings (ports and interfaces) are used while building Host and Configure is too late for it.
// TODO: Bind to Configuration["SiloProgram"], populate from appsettings.json instead of hardcoded C#?
// TODO: Split into subclasses
//  - ConsulServiceConfig
//  - RelativePortsConfig - not hardcoded.  Add some place to look up available ports.
public class SiloProgramOptions
{
    #region (Constants)

    public const string ConfigLocation = "SiloProgram";

    #endregion

    #region (static)

    public const int DefaultPortBase = 9090;

    #endregion

    #region Construction

    public SiloProgramOptions() : this(DefaultPortBase) { }

    public SiloProgramOptions(int portBase)
    {
        BasePort = portBase;
    }

    #endregion

    #region Interface and Port

    public int BasePort
    {
        get => portBase; 
        set
        {
            portBase = value;
            DashboardPort = BasePort;
            HttpPort = BasePort + 1;
            SiloPort = BasePort + 2;
            GatewayPort = BasePort + 3;
            OrleansHealthCheckPort = BasePort + 4;
            HttpsPort = BasePort + 5;
        }
    }
    private int portBase = 0;
    public string OrleansInterface { get; set; } = "127.0.0.1"; // TODO - configure
    public int SiloPort { get; set; }
    public int GatewayPort { get; set; }

    public int OrleansHealthCheckPort { get; set; }

    #endregion

    #region Web

    public string HttpInterface { get; set; } = "127.0.0.1";
    public int? HttpPort { get; set; }

    public string HttpsInterface { get; set; } = "127.0.0.1";
    public int? HttpsPort { get; set; }

    public string HttpUrls
    {
        get
        {
            var sb = new StringBuilder();
            if (HttpPort.HasValue)
            {
                sb.Append($"http://{HttpInterface}:{HttpPort}");
            }
            if (HttpsPort.HasValue)
            {
                if (sb.Length > 0) { sb.Append(";"); }
                sb.Append($"https://{HttpsInterface}:{HttpsPort}");
            }
            return sb.ToString();
        }
    }

    #endregion

    #region Dashboard

    public bool DashboardEnabled { get; set; } = true;
    public string DashboardInterface { get; set; } = "127.0.0.1";
    public int DashboardPort { get; set; }

    #endregion

    public IPEndPoint? LocalhostPrimaryClusterEndpoint { get; set; }


}
