using LionFire.Configuration;
using Microsoft.Extensions.Configuration;

namespace LionFire.ExtensionMethods.Configuration;

public static class IHasConfigLocationX
{
    public static T Bind<T>(this T options, IConfiguration configuration) where T : IHasConfigLocation
    {
        configuration.Bind(options.ConfigLocation, options);
        //Configuration.GetSection(ValorActionSiloOptions.ConfigLocation).Bind(Options);
        return options;
    }
}