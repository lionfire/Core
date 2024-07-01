using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace LionFire.AspNetCore;

// Based on: https://docs.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-6.0
public class TokenProvider
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }

    public JwtSecurityToken? Jwt
    {
        get
        {
            if (jwt == null && AccessToken != null)
            {
                jwt = new JwtSecurityTokenHandler().ReadJwtToken(AccessToken);
            }
            return jwt;
        }
    }
    private JwtSecurityToken? jwt;
 
}
