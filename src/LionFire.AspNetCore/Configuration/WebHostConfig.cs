using LionFire.Configuration;
using LionFire.Hosting;
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static LionFire.Net.PortConfiguration;
using LionFire.ExtensionMethods.Configuration;

namespace LionFire.AspNetCore;

public class WebHostConfig : IHasConfigLocation
{
    public static string DefaultConfigLocation => "WebHost";
    public virtual string ConfigLocation => DefaultConfigLocation;

    #region Injected via ctor

    protected int? BasePort { get; set; } // Inject this from LionFire.Net.PortsConfig

    #endregion

    #region Construction

    public WebHostConfig(IConfiguration configuration)
    {
        var portsConfig = new PortsConfig(configuration);
        BasePort = portsConfig.EffectiveBasePort;
        this.Bind(configuration);
    }

    #endregion


    /// <summary>
    /// If false, calls to WebHost() become a no-op. (Do not initialize AspNetCore and Kestrel)
    /// </summary>
    public bool Enabled { get; set; } = true;

    public bool HasAnyInterfaces => Http || Https;

    #region HTTP

    public bool Http { get; set; }
    public string? HttpInterface { get; set; } = "localhost";

    public int? HttpPort
    {
        get => GetPortOrOffset(httpPort, BasePort, HttpPortOffset);
        set => httpPort = value;
    }
    public int? httpPort = null;

    public int? HttpPortOffset { get; set; } = 0;

    #endregion

    #region HTTPS

    public bool Https { get; set; }
    public string? HttpsInterface { get; set; } = "localhost";
    public int? HttpsPortOffset { get; set; } = 1;
    public int? HttpsPort
    {
        get => GetPortOrOffset(httpsPort, BasePort, HttpsPortOffset);
        set => httpsPort = value;
    }
    public int? httpsPort = null;

    #endregion

    #region URLs

    /// <summary>
    /// This is in addition to the Urls provided when UseHttp or UseHttps are true.
    /// Default: none.
    /// e.g. new[] { "http://localhost:5000", "https://localhost:5001" }
    /// </summary>
    public List<string>? Urls { get; set; }

    #endregion

    public bool BlazorServer { get; set; }


    #region Program Requirements: set in derived classes

    public virtual bool RequiresAuth => true;
    public virtual bool RequiresBlazorServer => BlazorServer;

    public virtual bool RequiresControllers => false;
    public virtual bool RequiresStaticFiles => RequiresBlazorServer;

    public virtual bool RequiresMvc => false;
    public virtual bool RequiresMvcCore => false;

    public virtual bool RequiresControllersWithViews => false;

    public virtual bool RequiresRazorPages => RequiresBlazorServer;

    #endregion

    #region Derived

    public virtual bool HasAnyWebUI => RequiresBlazorServer;
    public virtual bool HasAnyFeatures => HasAnyWebUI || RequiresMvc || RequiresControllers || RequiresStaticFiles || RequiresRazorPages || RequiresBlazorServer;

    #endregion
}
