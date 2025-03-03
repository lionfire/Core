using LionFire.Configuration;
using LionFire.Hosting;
using LionFire.Net;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static LionFire.Net.PortConfiguration;
using LionFire.ExtensionMethods.Configuration;
using System.Reflection;

namespace LionFire.AspNetCore;

public interface IWebHostConfig
{
    //static  string DefaultConfigLocation => "WebHost";
    static abstract string DefaultConfigLocation { get; }
}
public class WebHostConfig : HasPortsConfigBase, IHasConfigLocation, IWebHostConfig
{
    #region Config binding

    public static string DefaultConfigLocation => "WebHost";
    public string ConfigLocation => DefaultConfigLocation;

    #endregion

    #region Lifecycle

    public WebHostConfig(IConfiguration configuration) : base(configuration)
    {
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

    public bool BlazorInteractiveServer { get; set; }
    public virtual Assembly[] AdditionalRazorAssemblies => [];


    #region Program Requirements: set in derived classes

    public virtual bool RequiresAuth => true;
    public virtual bool RequiresBlazorInteractiveServer => BlazorInteractiveServer;

    public virtual bool RequiresControllers => false;
    public virtual bool RequiresStaticFiles => RequiresBlazorInteractiveServer;

    public virtual bool RequiresMvc => false;
    public virtual bool RequiresMvcCore => false;

    public virtual bool RequiresControllersWithViews => false;

    public virtual bool RequiresRazorPages => false;

    #endregion

    /// <summary>
    /// Flag the application can set to globally enable user-interactive UI
    /// </summary>
    public bool WebUI { get; set; }

    #region Derived

    public virtual bool HasAnyWebUI => RequiresBlazorInteractiveServer || WebUI;
    public virtual bool HasAnyFeatures => HasAnyWebUI || RequiresMvc || RequiresControllers || RequiresStaticFiles || RequiresRazorPages || RequiresBlazorInteractiveServer;

    #endregion
}
