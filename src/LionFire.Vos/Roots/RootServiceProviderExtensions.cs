using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Vos
{
    public static class RootServiceProviderExtensions
    {
        public static RootVob GetRootVob(this IServiceProvider services, string name = null)
            => services.GetRequiredService<RootManager>().Get(name);
    }
}
