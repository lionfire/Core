#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using LionFire.Applications.Updates;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting
{
    public static class UpdaterHostingExtensions
    {
        public static IServiceCollection AddUpdater<TUpdater>(this IServiceCollection services)
            where TUpdater : class, IUpdater
            => services
                .AddSingleton<UpdaterService>()
                .AddSingleton<IUpdater, TUpdater>()
                ;
    }
}
