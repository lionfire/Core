using LionFire.Auth.OpenIddict.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Client;
using System;

namespace LionFire.Hosting;

public static class OpenIddictClientWebIntegrationBuilderX
{
    public static OpenIddictClientWebIntegrationBuilder AddLionFire(this OpenIddictClientWebIntegrationBuilder builder, Action<ConfigurableLionFireIdRegistration>? configuration = null, params string[] scopes)
    {
        builder.Services.Configure(delegate (OpenIddictClientOptions options)
        {
            var DefaultRedirectionEndpointUri = "/signin-oidc";
            if (options.RedirectionEndpointUris.Count == 0)
            {
                options.RedirectionEndpointUris.Add(new Uri(DefaultRedirectionEndpointUri, UriKind.RelativeOrAbsolute));
            }

            OpenIddictClientRegistration openIddictClientRegistration = new OpenIddictClientRegistration
            {
                ProviderSettings = new LionFireIdProviderSettings(),
                Issuer = new Uri("https://id.lionfire.ca"),

                #region Set by application
                //ClientId = ..., // Set by application
                //ClientSecret = , // If required (not useful for public clients, can be useful for internally-hosted servers)
                //RedirectUri = new Uri(), 
                #endregion
                ProviderName = "LionFire"

            };

            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    openIddictClientRegistration.Scopes.Add(scope);
                }
            }
            openIddictClientRegistration.Scopes.Add("openid");
            openIddictClientRegistration.Scopes.Add("profile");
            openIddictClientRegistration.Scopes.Add("email");

            configuration?.Invoke(new ConfigurableLionFireIdRegistration(openIddictClientRegistration));
            options.Registrations.Add(openIddictClientRegistration);
        });
        return builder;
    }
}
