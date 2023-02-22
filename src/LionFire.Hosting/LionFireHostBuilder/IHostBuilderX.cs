#nullable enable
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System;
using LionFire.ExtensionMethods;

namespace LionFire.Hosting;
public static class IHostBuilderX
{   
    /// <summary>
    /// Fluent builder for LionFire's initialization of IHostBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="action"></param>
    /// <param name="useDefaults"></param>
    /// <returns></returns>
    public static IHostBuilder LionFire(this IHostBuilder hostBuilder, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    {
        var lf = (ILionFireHostBuilder) hostBuilder.Properties.GetOrAdd(typeof(LionFireHostBuilder).Name, _ =>
        {
            var result = new LionFireHostBuilder(hostBuilder);
            if (useDefaults) { result.Defaults(); }
            return result;
        });

        action?.Invoke(lf);

        return hostBuilder;
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="config"></param>
    ///// <param name="args"></param>
    ///// <param name="defaultBuilder">If true, and hostBuilder is null, initializes IHostBuilder from Host.CreateDefaultBuilder(args).  
    ///// Also, if true, adds default LionFire services:
    /////  - AddLionFireLogging(config)
    /////  - SetAsDefaultServiceProvider()
    ///// </param>
    ///// <param name="hostBuilder"></param>
    ///// <returns></returns>
    //[Obsolete]
    //public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true)
    //{
    //    return (defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder())
    //        .AddLionFire();
    //}

    // OLD
    //[Obsolete("Use .LionFire()")]
    //public static IHostBuilder AddLionFire(this IHostBuilder hostBuilder)
    //    => hostBuilder
    //        .AddLionFireLogging()
    //        .SetAsDefaultServiceProvider()
    //        //.ConfigureContainer<LionFireDefaultServiceProviderFactory>(f => { })
    //        .UseServiceProviderFactory(new LionFireDefaultServiceProviderFactory())
    //        ;

#if FUTURE // Is this really needed?
    // REVIEW - adapted from Microsoft
    public static IHostBuilder AddDefaultBuilderForMaui(this IHostBuilder hostBuilder, string[] args = null)
    {
        hostBuilder.ConfigureAppConfiguration(delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            IHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
            //bool reloadOnChange = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
            bool reloadOnChange = false;
            config
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange)
                .AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, reloadOnChange);
            if (hostingEnvironment.IsDevelopment() && !string.IsNullOrEmpty(hostingEnvironment.ApplicationName))
            {
                Assembly assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
                if (assembly != null)
                {
                    config.AddUserSecrets(assembly, optional: true);
                }
            }

            config.AddEnvironmentVariables();
            if (args != null)
            {
                config.AddCommandLine(args);
            }
        });
        return hostBuilder;
    }
#endif
}
