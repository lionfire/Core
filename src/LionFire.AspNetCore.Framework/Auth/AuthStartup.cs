using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.AspNetCore;

public static class AuthStartup
{

    public static IServiceCollection TryAddDefaultAuthentication(this IServiceCollection services, WebHostConfig config)
    {
        if (config.RequiresAuth)
        {
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "LionFire";
                //options.DefaultChallengeScheme = "oidc";
            })
                // TODO: Also compare with Microsoft.AspNetCore.Authentication.JwtBearer nuget
                .AddCookie("Cookies")
                //.ServerWebUI_StandardOidcClient()
                ;
            //services.AddAuthorization(o =>
            //{
            //    o.AddPolicy("default", new Microsoft.AspNetCore.Authorization.AuthorizationPolicy())
            //});
        }

        return services;
    }

}
