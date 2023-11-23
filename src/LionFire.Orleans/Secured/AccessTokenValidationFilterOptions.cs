// Retrieved on 220613 from: Apache license
// https://raw.githubusercontent.com/MV10/OrleansJWT/6e96b33c66f2c992a27b7dbf3be796546f89b4c3/SecuredGrain/AccessTokenValidationFilter.cs
// https://github.com/MV10/OrleansJWT

namespace LionFire.Orleans_;

public class AccessTokenValidationFilterOptions
{
    public string Audience { get; set; }
    public bool UseIntrospection { get; set; }
    public string Authority { get; set; }
    public string Issuer { get; set; }
}
