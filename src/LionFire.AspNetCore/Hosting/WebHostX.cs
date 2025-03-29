using LionFire.Deployment;
using LionFire.Hosting;
using LionFire.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using LionFire.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LionFire.ExtensionMethods.Dependencies;
using Microsoft.Extensions.Options;
using LionFire.ExtensionMethods.Configuration;

namespace LionFire.Hosting;

public static class WebHostConfigX
{
    public static WebHostConfig? GetWebHostConfig(this HostBuilderContext? context) => (context?.Properties.TryGetValue(typeof(WebHostConfig).Name) as Func<WebHostConfig>)?.Invoke();
    //public static Func<WebHostConfig>? GetWebHostConfigFunc(this HostBuilderContext? context) => (context?.Properties.TryGetValue(typeof(WebHostConfig).Name) as Func<WebHostConfig>);

    public static TWebHostConfig GetWebHostConfig<TWebHostConfig>(this IConfiguration configuration, Action<TWebHostConfig>? configure = null)
        where TWebHostConfig : WebHostConfig
    {
        var result = (TWebHostConfig)(Activator.CreateInstance(typeof(TWebHostConfig), configuration)
            ?? throw new Exception($"Failed to create {typeof(TWebHostConfig).FullName}.  It must have a constructor accepting a single parameter of type IConfiguration."));
        configure?.Invoke(result);
        return result;
    }

    public static Func<IConfiguration, WebHostConfig> GetWebHostConfigFunc<TWebHostConfig>()
        where TWebHostConfig : WebHostConfig
        => configuration => (WebHostConfig)(Activator.CreateInstance(typeof(TWebHostConfig), configuration)
            ?? throw new Exception($"Failed to create {typeof(TWebHostConfig).FullName}.  It must have a constructor accepting a single parameter of type IConfiguration."));
}

public class LionFireWebHostBuilder
{
    public ILionFireHostBuilder Builder { get; }

    public LionFireWebHostBuilder(ILionFireHostBuilder builder)
    {
        Builder = builder;
    }

    public List<Action<WebHostConfig>> Actions { get; } = new();
}

public static class LionFireWebHostBuilderX
{
    public static string prefix => WebHostConfig.DefaultConfigLocation;

    public static LionFireWebHostBuilder RootComponent<T>(this LionFireWebHostBuilder lfw)
        => lfw.Configure(c => c.RootComponent = typeof(T));
    // ENH - maybe
    public static LionFireWebHostBuilder Startup<T>(this LionFireWebHostBuilder lfw)
        => lfw.Configure(c => c.Startup = typeof(T));
    // ENH - maybe
    public static LionFireWebHostBuilder Configuration<T>(this LionFireWebHostBuilder lfw)
        => lfw.Configure(c => c.Configuration = typeof(T));

    public static LionFireWebHostBuilder Configure(this LionFireWebHostBuilder lfw, Action<WebHostConfig> action)
    {
        lfw.Actions.Add(action);
        return lfw;
    }

    public static LionFireWebHostBuilder Http(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Http)}", enabled.ToString())]); return lfw; }

    #region TODO ENH - instead of setting Configuration key/value pairs, store a set of Action<WebHostConfig> configure methods, and execute them after WebHostConfig is instantiated.

    public static LionFireWebHostBuilder Http_ENH(this LionFireWebHostBuilder lfw, bool enabled = true) => lfw.Configure(whc => whc.Http = enabled);

    public static LionFireWebHostBuilder Https(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Https)}", enabled.ToString())]); return lfw; }
    //[Obsolete("Use BlazorInteractiveServer")]
    //public static LionFireWebHostBuilder BlazorServer(this LionFireWebHostBuilder lfw, bool enabled = true) => lfw.BlazorInteractiveServer(enabled);
    public static LionFireWebHostBuilder BlazorInteractiveServer(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.BlazorInteractiveServer)}", enabled.ToString())]); return lfw; }

    public static LionFireWebHostBuilder WebUI(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.WebUI)}", enabled.ToString())]); return lfw; }

    //public static LionFireWebHostBuilder Mvc(this LionFireWebHostBuilder lfw, bool enabled = true)
    //{ lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Mvc)}", enabled.ToString())]); return lfw; }

    #endregion
}

public static class WebHostX
{
    public static ILionFireHostBuilder WebHost<TStartup, TWebHostConfig, TRootComponent>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder> configure
        //, Action<TWebHostConfig>? configureWebHostConfig = null
        )
        where TStartup : class
        where TWebHostConfig : WebHostConfig, IWebHostConfig
        where TRootComponent : Microsoft.AspNetCore.Components.ComponentBase
        => builder.WebHost<TStartup, TWebHostConfig>(configure: webHostBuilder =>
        {
            webHostBuilder.Configure(c => c.RootComponent = typeof(TRootComponent));
            configure?.Invoke(webHostBuilder);
        });

    public static ILionFireHostBuilder WebHost<TStartup, TWebHostConfig>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure
        )
        where TStartup : class
        where TWebHostConfig : WebHostConfig, IWebHostConfig
    {
        var webHostBuilder = new LionFireWebHostBuilder(builder);
        configure?.Invoke(webHostBuilder);

        var webHostConfig = builder.Configuration.GetWebHostConfig<TWebHostConfig>();

        builder.HostBuilder.Properties[typeof(WebHostConfig).Name] = webHostConfig;

        builder.ConfigureServices(s => s.AddSingleton(webHostConfig));

        return builder.ApplyWebHostConfig<TStartup>(webHostConfig);
    }

    // Convenience overload: TWebHostConfig defaults to WebHostConfig
    public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure = null)
        where TStartup : class
            => builder.WebHost<TStartup, WebHostConfig>(configure);
    //throw new NotImplementedException("FIXME - config not being wired up for some reason");// builder.WebHost<TStartup, WebHostConfig>();

    private static ILionFireHostBuilder ApplyWebHostConfig<TStartup>(this ILionFireHostBuilder builder, WebHostConfig webHostConfig)
        where TStartup : class
    {
        if (webHostConfig.NeedsWebHost) // REVIEW - webHostConfig is not otherwise used here
        {
            builder.HostBuilder.ConfigureWebHostDefaults(webBuilder =>
                {
                    #region URLs

                    var applicationName = builder.Configuration[WebHostDefaults.ApplicationKey];

                    webBuilder
                        .UseStartup<TStartup>()
                        //.UseContentRoot(AppContext.BaseDirectory)
                        .UseSetting(WebHostDefaults.ApplicationKey, applicationName)  // Undo the "applicationName" change.
                        ;

                    var urls = GetConfiguredUrls(builder.HostBuilder.Configuration);
                    if (urls.Length > 0)
                    {
                        webBuilder.UseUrls(urls.ToArray());
                    } // else it falls back to Kestrel's own configuration

                    #endregion
                });
        }
        return builder;
    }

    private static string[] GetConfiguredUrls(IConfiguration configuration)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#kestrel-endpoint-configuration
        // https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0

        var o = new WebHostConfig(configuration);

        HashSet<string> urls = new();

        if (o.Http && o.HttpPort.HasValue && o.HttpInterface != null)
        {
            urls.Add($"http://{o.HttpInterface}:{o.HttpPort.Value}");
        }

        if (o.Https && o.HttpsPort.HasValue && o.HttpsInterface != null)
        {
            urls.Add($"https://{o.HttpsInterface}:{o.HttpsPort.Value}");
        }

        if (o.Urls != null)
        {
            foreach (var url in o.Urls)
            {
                if (urls.Contains(url))
                {
                    urls.Add(url);
                }
            }
        }
        return urls.ToArray();
    }
}
