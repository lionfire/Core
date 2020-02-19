using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Vos
{
    public static class RootServiceProviderExtensions
    {
        public static IRootVob GetRootVob(this IServiceProvider services, string name = null)
            => services.GetRequiredService<IRootManager>().Get(name);
    }
}
