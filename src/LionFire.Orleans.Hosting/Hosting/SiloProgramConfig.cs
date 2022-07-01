#nullable enable
using System.Reflection;
using System.Text;

namespace LionFire.Hosting;

//public class ConsulServiceOptions
//{

//}

public class SiloProgramConfig
{
    #region (static)

    public const int DefaultPortBase = 9090;

    #endregion

    #region Construction

    public SiloProgramConfig() : this(DefaultPortBase) { }

    public SiloProgramConfig(int portBase)
    {
        PortBase = portBase;
        DashboardPort = PortBase;
        HttpPort = PortBase + 1;
        SiloPort = PortBase + 2;
        GatewayPort = PortBase + 3;
        OrleansHealthCheckPort = PortBase + 4;
        HttpsPort = PortBase + 5;
    }

    #endregion



    #region Consul

    //public ConsulServiceOptions ConsulServiceOptions { get; set; }

    public string ConsulDatacenter = "dc1";
    public bool RegisterSiloWithConsul { get; set; } = true;


    #endregion

    #region Interface and Port

    public int PortBase { get; set; } = 0;

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

    public bool OrleansDashboard { get; set; } = true;
    public string DashboardInterface { get; set; } = "127.0.0.1";
    public int DashboardPort { get; set; }

    #endregion

}
