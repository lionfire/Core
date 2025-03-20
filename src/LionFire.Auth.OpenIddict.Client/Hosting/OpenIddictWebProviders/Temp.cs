#error NEXT
using global::OpenIddict.Client;
using System.Net.Http.Json;
using static OpenIddict.Client.AspNetCore.OpenIddictClientAspNetCoreConstants;
using static OpenIddict.Client.OpenIddictClientEvents;

namespace LionFire.Auth.OpenIddict.Client.Hosting.OpenIddictWebProviders;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Client;
//using OpenIddict.Client.AspNetCore;
using static global::OpenIddict.Abstractions.OpenIddictConstants;
//using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

public class HandleCodeExchangeHandler : IOpenIddictClientHandler<ProcessChallengeContext>
{
    private readonly HttpClient _httpClient;

    public HandleCodeExchangeHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask HandleAsync( context)
    {

    }
    public async ValueTask HandleAsync(ProcessChallengeContext context)
    {
        var httpContext = context.Transaction.GetHttpRequest()?.HttpContext;
        if (httpContext == null) return;

        // Check if this is the redirect with the code
        var code = httpContext.Request.Query["code"].FirstOrDefault();
        if (string.IsNullOrEmpty(code)) return;

        Console.WriteLine($"Received code: {code}");

        // Exchange the code for tokens
        var request = new HttpRequestMessage(HttpMethod.Post, context.Issuer + "/connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = httpContext.Request.GetAbsoluteUri("/signin-oidc"),

            //["client_id"] = context.Options.ClientId,
            //["client_secret"] = context.Options.ClientSecret
        });

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Token exchange failed: {error}");
            throw new InvalidOperationException("Failed to exchange code for tokens.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var idToken = tokenResponse?.GetValueOrDefault("id_token");
        var accessToken = tokenResponse?.GetValueOrDefault("access_token");

        Console.WriteLine($"ID Token: {idToken}");
        Console.WriteLine($"Access Token: {accessToken}");

        // Store tokens in the transaction
        context.Transaction.SetProperty(Parameters.IdToken, idToken);
        context.Transaction.SetProperty(Parameters.AccessToken, accessToken);

        context.HandleRequest(); // Stop further processing, let subsequent handlers take over
    }
}

public static class HttpRequestExtensions
{
    public static string GetAbsoluteUri(this HttpRequest request, string relativePath)
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Path = relativePath
        };

        if (request.Host.Port.HasValue)
        {
            uriBuilder.Port = request.Host.Port.Value;
        }

        return uriBuilder.Uri.ToString();
    }
}
