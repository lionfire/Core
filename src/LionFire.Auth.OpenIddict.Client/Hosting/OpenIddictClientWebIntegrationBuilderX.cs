using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using LionFire.Auth.OpenIddict.Client;

namespace LionFire.Hosting;

public static class OpenIddictClientWebIntegrationBuilderX
{
    public static OpenIddictClientWebIntegrationBuilder AddLionFire(this OpenIddictClientWebIntegrationBuilder builder, Action<ConfigurableLionFireIdRegistration>? configuration = null)
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
                Issuer = new Uri("https://id.lionfire.ca"),

                #region Set by application
                //ClientId = ..., // Set by application
                //ClientSecret = , // If required (not recommended but doesn't hurt)
                //RedirectUri = new Uri(), 
                #endregion

                ProviderName = "LionFire"

            };
            openIddictClientRegistration.Scopes.Add("openid");
            openIddictClientRegistration.Scopes.Add("profile");
            openIddictClientRegistration.Scopes.Add("email");
            openIddictClientRegistration.Scopes.Add("valor");
            openIddictClientRegistration.Scopes.Add("valor.client");

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