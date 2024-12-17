using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using LionFire.Auth.OpenIddict.Client;

namespace LionFire.Hosting;

public static class OpenIddictClientWebIntegrationBuilderX
{
    public static OpenIddictClientWebIntegrationBuilder AddLionFireId(this OpenIddictClientWebIntegrationBuilder builder, Action<ConfigurableLionFireIdRegistration>? configuration = null)
    {
        Action<ConfigurableLionFireIdRegistration>? configuration2 = configuration;
        //if (configuration2 == null)
        //{
        //    throw new ArgumentNullException("configuration");
        //}

        builder.Services.Configure(delegate (OpenIddictClientOptions options)
        {
            OpenIddictClientRegistration openIddictClientRegistration = new OpenIddictClientRegistration
            {
                ProviderSettings = new LionFireIdProviderSettings(),
                //ProviderType = "87edae0b-e71e-4163-960f-cf7e2a780d77" // REVIEW - what's this?
            };
            if (configuration2 != null)
            {
                configuration2(new ConfigurableLionFireIdRegistration(openIddictClientRegistration));
            }
            options.Registrations.Add(openIddictClientRegistration);
        });
        return builder;
    }

    //    /// <summary>
    //    /// Configure the loggers
    //    /// </summary>
    //    /// <param name="hostBuilder">IHostBuilder</param>
    //    /// <returns>IHostBuilder</returns>
    //    public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
    //    {
    //        return hostBuilder.ConfigureLogging((hostContext, configLogging) =>
    //        {
    //            configLogging
    //                .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
    //                .AddConsole()
    //                .AddDebug();
    //        });
    //    }
}