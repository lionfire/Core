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

    public static TWebHostConfig GetWebHostConfig<TWebHostConfig>(this IConfiguration configuration)
        where TWebHostConfig : WebHostConfig
        => (TWebHostConfig)(Activator.CreateInstance(typeof(TWebHostConfig), configuration)
            ?? throw new Exception($"Failed to create {typeof(TWebHostConfig).FullName}.  It must have a constructor accepting a single parameter of type IConfiguration."));

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
}

public static class LionFireWebHostBuilderX
{
    public static string prefix => WebHostConfig.DefaultConfigLocation;

    public static LionFireWebHostBuilder RootComponent<T>(this LionFireWebHostBuilder lfw)
    {
        lfw.RootComponent = typeof(T);
        return lfw;
    }

    public static LionFireWebHostBuilder Http(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Http)}", enabled.ToString())]); return lfw; }
    public static LionFireWebHostBuilder Https(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Https)}", enabled.ToString())]); return lfw; }
    [Obsolete("Use BlazorInteractiveServer")]
    public static LionFireWebHostBuilder BlazorServer(this LionFireWebHostBuilder lfw, bool enabled = true) => lfw.BlazorInteractiveServer(enabled);
    public static LionFireWebHostBuilder BlazorInteractiveServer(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.BlazorInteractiveServer)}", enabled.ToString())]); return lfw; }

    public static LionFireWebHostBuilder WebUI(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.WebUI)}", enabled.ToString())]); return lfw; }

    //public static LionFireWebHostBuilder Mvc(this LionFireWebHostBuilder lfw, bool enabled = true)
    //{ lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Mvc)}", enabled.ToString())]); return lfw; }

}

public static class WebHostX
{
    public static ILionFireHostBuilder WebHost<TStartup, TWebHostConfig, TRootBlazorComponent>(this ILionFireHostBuilder builder, Action<TWebHostConfig>? configureWebHostConfig = null)
        where TStartup : class
        where TWebHostConfig : WebHostConfig, IWebHostConfig
        => builder.WebHost<TStartup, TWebHostConfig>(configureWebHostConfig: whc =>
        {
            whc.RootComponent = typeof(TRootBlazorComponent);
            configureWebHostConfig?.Invoke(whc);
        });

#error LionFire.All: see if Action<LionFireWebHostBuilder> is used at all. If not, eliminate in favor of Action<TWebHostConfig>.
    public static ILionFireHostBuilder WebHost<TStartup, TWebHostConfig>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure = null, Action<TWebHostConfig>? configureWebHostConfig = null)
        where TStartup : class
        where TWebHostConfig : WebHostConfig, IWebHostConfig
    {
        var w = new LionFireWebHostBuilder(builder);
        configure?.Invoke(w);

        builder.HostBuilder.Properties[typeof(WebHostConfig).Name] = () => builder.Configuration.GetWebHostConfig<TWebHostConfig>();

        var webHostConfig = builder.Configuration.GetWebHostConfig<TWebHostConfig>();

        configureWebHostConfig?.Invoke(webHostConfig);

        return builder._WebHost<TStartup>(webHostConfig);
    }

    // Convenience overload: TWebHostConfig defaults to WebHostConfig
    public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure = null)
        where TStartup : class
            => builder.WebHost<TStartup, WebHostConfig>(configure);
    //throw new NotImplementedException("FIXME - config not being wired up for some reason");// builder.WebHost<TStartup, WebHostConfig>();

    private static ILionFireHostBuilder _WebHost<TStartup>(this ILionFireHostBuilder builder, WebHostConfig o /*Func<IConfiguration, WebHostConfig> oFunc*/)
        where TStartup : class
    {
        //{
        //var o = oFunc(builder.Configuration);
        if (!o.Enabled || !o.HasAnyInterfaces || !o.HasAnyFeatures) return builder;
        //}

        builder.HostBuilder
            .ConfigureWebHostDefaults(webBuilder =>
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
