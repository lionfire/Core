using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;

namespace LionFire.Auth.Controllers;

[ApiController]
public class AuthController : Controller
{
    [HttpGet("/auth/login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            // Note: when only one client is registered in the client options,
            // specifying the issuer URI or the provider name is not required.
            [OpenIddictClientAspNetCoreConstants.Properties.ProviderName] = "LionFire"
        })
        {
            RedirectUri = "/",
        };
        
        return Challenge(properties, global::OpenIddict.Client.AspNetCore.OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    }
}
