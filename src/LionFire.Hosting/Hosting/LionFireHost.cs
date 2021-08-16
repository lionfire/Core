using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace LionFire.Hosting
{
    public static class LionFireHost
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="args"></param>
        /// <param name="defaultBuilder">If true, and hostBuilder is null, initializes IHostBuilder from Host.CreateDefaultBuilder(args).  
        /// Also, if true, adds default LionFire services:
        ///  - AddLionFireLogging(config)
        ///  - SetAsDefaultServiceProvider()
        /// </param>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IHostBuilder Create(IConfiguration config = null, string[] args = null, bool defaultBuilder = true, IHostBuilder hostBuilder = null)
        {
            if (config == null && defaultBuilder)
            {
                config = HostConfiguration.CreateDefault();
            }
            if (hostBuilder == null)
            {
                hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();
            }

            return (hostBuilder)
                .If(defaultBuilder, b =>
                b
                    .AddLionFireLogging(config)
                    .SetAsDefaultServiceProvider()
                )

                //.ConfigureContainer<LionFireDefaultServiceProviderFactory>(f => { })
                .UseServiceProviderFactory(new LionFireDefaultServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    //services.AddSingleton<IServiceProviderFactory<>>
                });
        }


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
    }
}
