using LionFire.Dependencies;
using System;

namespace LionFire.Vos.Services
{
    public static class IVobServiceExtensions
    {
        public static T GetService<T>(this IVob vob) where T : class 
            => (T)vob.GetNext<IServiceProvider>()?.GetService(typeof(T));

        public static T GetRequiredService<T>(this IVob vob)
        {
            var serviceProvider = vob.GetNextRequired<IServiceProvider>();
            var result = (T)serviceProvider.GetService(typeof(T));
            if (result == null) throw new HasUnresolvedDependenciesException($"{typeof(T).FullName} not service not available for vob '{vob}'");
            return result;
        }
    }
}
