using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.Runtime;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// OPTIMIZE: Use Cysharp LitJwt 
// TODO: Review / cleanup this file

// Based on this, retrieved on 220613 (Apache license)
// https://raw.githubusercontent.com/MV10/OrleansJWT/6e96b33c66f2c992a27b7dbf3be796546f89b4c3/SecuredGrain/AccessTokenValidationFilter.cs
// https://github.com/MV10/OrleansJWT
// Modifications:
//  - added local validation of signature
//  - options

namespace LionFire.Orleans_;

public class AccessTokenValidationFilter : IIncomingGrainCallFilter
{
    private readonly IMemoryCache memoryCache;
    private readonly IDiscoveryCache discoveryCache;
    private readonly IHttpClientFactory httpClientFactory;

    private string? Audience { get; }
    public bool UseIntrospection { get; }

    TokenValidationParameters validationParameters { get; }
    ConfigurationManager<OpenIdConnectConfiguration>? configurationManager; // Used to get issuer signing keys
    protected AccessTokenValidationFilterOptions Options { get; }

    public AccessTokenValidationFilter(
        IMemoryCache memoryCache,
        IDiscoveryCache discoveryCache,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<AccessTokenValidationFilterOptions> options)
    {
        this.memoryCache = memoryCache;
        this.discoveryCache = discoveryCache;
        this.httpClientFactory = httpClientFactory;
        this.Audience = options.CurrentValue.Audience;
        this.UseIntrospection = options.CurrentValue.UseIntrospection;
        this.Options = options.CurrentValue;

        validationParameters = new()
        {
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuer = true,
            ValidIssuer = Options.Issuer,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidAudience = Options.Audience,
            // Allow for some drift in server time
            // (a lower value is better; we recommend two minutes or less)
            ClockSkew = TimeSpan.FromMinutes(2),
            // See additional validation for aud below
        };
    }

    public async Task Invoke(IIncomingGrainCallContext context)
    {
        var securedResponseType = GetSecuredResponseType();

        if (securedResponseType == null)
        {
            await context.Invoke();
            return;
        }

        context.Result = Activator.CreateInstance(securedResponseType);

        try
        {
            var accessToken = (string)RequestContext.Get("Bearer");
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new Exception("Unauthorized (bearer token missing in RequestContext)");

            var jwt = await GetValidToken(accessToken);

            RequestContext.Set("sub", jwt.Subject);
            RequestContext.Set("jwt", jwt);

            await context.Invoke();
            SetSecuredResponse(true, "Authorized");
        }
        catch (Exception ex)
        {
            SetSecuredResponse(false, $"{ex.GetType().Name}: {ex.Message}");
        }

        Type GetSecuredResponseType()
        {
            // always a Task in Orleans but not always generic
            var task = context.ImplementationMethod.ReturnType;
            if (!task.IsGenericType) return null;
            if (!task.GetGenericTypeDefinition().Equals(typeof(Task<>))) return null;
            // assume nothing goofy like Task<string, SecuredResponse<>> etc.
            var taskArg = task.GetGenericArguments()[0];
            if (!taskArg.IsGenericType) return null;
            if (!taskArg.GetGenericTypeDefinition().Equals(typeof(SecuredResponse<>))) return null;
            return taskArg;
        }

        void SetSecuredResponse(bool success, string message)
        {
            ((ISecuredResponseValidation)context.Result).Success = success;
            ((ISecuredResponseValidation)context.Result).Message = message;
        }
    }

    private string GetCacheKey(JwtSecurityToken jwt)
        => $"accesstokensid:{jwt.Subject}";

    private async Task<JwtSecurityToken> GetValidToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadToken(accessToken) as JwtSecurityToken;

        try
        {
            if (jwt.ValidTo <= DateTime.Now)
                throw new Exception("Unauthorized (token expired)");

            if (!string.IsNullOrWhiteSpace(Audience) && !jwt.Audiences.Any(a => a.Equals(Audience)))
                throw new Exception($"Unauthorized ({Audience} scope required)");

            if (!jwt.Header.ContainsKey("typ")
                || !jwt.Header["typ"].Equals("at+jwt"))
                throw new Exception("Unauthorized (wrong token type)");
        }
        catch
        {
            //var key = GetCacheKey(jwt);
            // REVIEW: Commented this, as it could degrade performance (denial-of-service) for logged-in users
            //memoryCache.Remove(key);
            throw;
        }

        if (!IsKnownToken(jwt))
        {
            await VerifyAndCacheToken(jwt, handler);
        }
        return jwt;
    }

    private bool IsKnownToken(JwtSecurityToken jwt)
    {
        var key = GetCacheKey(jwt);
        if (!memoryCache.TryGetValue(key, out var cachedValue)) return false;
        if (!cachedValue.Equals(jwt.RawData))
        {
            memoryCache.Remove(key);
            return false;
        }
        return true;
    }

    private async Task LocallyValidateSignatureUsingServerKeys(JwtSecurityToken jwt, JwtSecurityTokenHandler handler, CancellationToken cancellationToken = default)
    {
        // Reference: https://developer.okta.com/code/dotnet/jwt-validation/#what-you-need

        if (validationParameters.IssuerSigningKeys == null)
        {
            if (string.IsNullOrEmpty(jwt?.RawData)) throw new ArgumentNullException(nameof(jwt.RawData));
            if (string.IsNullOrEmpty(Options?.Issuer)) throw new ArgumentNullException(nameof(Options.Issuer));

            configurationManager ??= new ConfigurationManager<OpenIdConnectConfiguration>(
                Options.Authority.TrimEnd('/') + "/.well-known/oauth-authorization-server",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
            validationParameters.ConfigurationManager ??= configurationManager;

            var discoveryDocument = await configurationManager.GetConfigurationAsync(cancellationToken);
            validationParameters.IssuerSigningKeys = discoveryDocument.SigningKeys;
        }

        handler.ValidateToken(jwt.RawData, validationParameters, out var rawValidatedToken);
    }

    private async Task Introspect(JwtSecurityToken jwt)
    {
        var discovery = await discoveryCache.GetAsync();
        if (discovery.IsError)
            throw new Exception("Unauthorized (authority discovery failed)");

        var client = httpClientFactory.CreateClient();

        // this user/pass combo is specific to the IdentityServer demo authority,
        // and the use of scope as the username is generally specific to IdentityServer
        client.SetBasicAuthenticationOAuth("api", "secret");

        throw new NotImplementedException("TODO: Client id/secret config");

        var tokenResponse = await client.IntrospectTokenAsync(
            new TokenIntrospectionRequest
            {
                Address = discovery.IntrospectionEndpoint,
                ClientId = "interactive.confidential", // TODO
                ClientSecret = "secret", // TODO
                Token = jwt.RawData
            });
        if (tokenResponse.IsError)
            throw new Exception($"Unauthorized (introspection error: {tokenResponse.Error})");

        if (!tokenResponse.IsActive)
            throw new Exception("Unauthorized (token deactivated by authority)");
    }

    private async Task VerifyAndCacheToken(JwtSecurityToken jwt, JwtSecurityTokenHandler handler, CancellationToken cancellationToken = default)
    {
        if (UseIntrospection)
        {
            await Introspect(jwt);
        }
        else
        {
            await LocallyValidateSignatureUsingServerKeys(jwt, handler, cancellationToken);
        }

        var key = GetCacheKey(jwt);
        memoryCache.Set(key, jwt.RawData);
    }
}