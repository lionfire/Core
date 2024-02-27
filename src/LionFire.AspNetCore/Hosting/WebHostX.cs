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

namespace LionFire.Hosting;

public static class WebHostConfigX
{
    public static WebHostConfig? GetWebHostConfig(this HostBuilderContext? context) => (context?.Properties.TryGetValue(typeof(WebHostConfig).Name) as Func<WebHostConfig>)?.Invoke();
    //public static Func<WebHostConfig>? GetWebHostConfigFunc(this HostBuilderContext? context) => (context?.Properties.TryGetValue(typeof(WebHostConfig).Name) as Func<WebHostConfig>);

    public static TWebHostConfig GetConfig<TWebHostConfig>(this IConfiguration configuration)
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
    private static string prefix => WebHostConfig.DefaultConfigLocation;

    public static LionFireWebHostBuilder Http(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Http)}", enabled.ToString())]); return lfw; }
    public static LionFireWebHostBuilder Https(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.Https)}", enabled.ToString())]); return lfw; }
    public static LionFireWebHostBuilder BlazorServer(this LionFireWebHostBuilder lfw, bool enabled = true)
    { lfw.Builder.ConfigureDefaults([new($"{prefix}:{nameof(WebHostConfig.BlazorServer)}", enabled.ToString())]); return lfw; }

}

public static class WebHostX
{
    public static ILionFireHostBuilder WebHost<TStartup, TWebHostConfig>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure = null)
        where TStartup : class
        where TWebHostConfig : WebHostConfig
    {
        var w = new LionFireWebHostBuilder(builder);
        configure?.Invoke(w);

        builder.HostBuilder.Properties[typeof(WebHostConfig).Name] = () => builder.Configuration.GetConfig<TWebHostConfig>();

        return builder._WebHost<TStartup>(builder.Configuration.GetConfig<TWebHostConfig>());
    }

    // Convenience overload: TWebHostConfig defaults to WebHostConfig
    public static ILionFireHostBuilder WebHost<TStartup>(this ILionFireHostBuilder builder, Action<LionFireWebHostBuilder>? configure = null)
        where TStartup : class
            => builder.WebHost<TStartup, WebHostConfig>();
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

                    var urls = GetConfiguredUrls(builder.HostBuilder.Configuration);

                    var applicationName = builder.Configuration[WebHostDefaults.ApplicationKey];

                    webBuilder
                        .UseStartup<TStartup>()
                        //.UseContentRoot(AppContext.BaseDirectory)
                        .UseSetting(WebHostDefaults.ApplicationKey, applicationName)  // Undo the "applicationName" change.
                        ;

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
