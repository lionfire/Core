using LionFire.DependencyMachines;
using LionFire.Persisters.Expanders;
using LionFire.Vos;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class VosFrameworkHostingExtensions
{
    public static IServiceCollection AddVosArchives(this IServiceCollection services, string path)
            => services
        .Configure<VosOptions>(vo =>
        {
            vo.PrimaryRootInitializers.Add(root =>
            {
                var list = new List<IParticipant>
                {
                    new Participant(key: "VosArchives for " + path)
                    {
                        StartAction = () =>
                        {
                            (root[path] as IVobInternals).GetOrAddVobNode<ExpanderMounter>();
                        }
                    }
                };
                return list;
            });
        })
                //.TryAddEnumerableSingleton<IVosPlugin >()
                ;
}


