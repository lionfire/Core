using LionFire.DependencyMachines;
using LionFire.Vos;
using LionFire.Vos.Internals;
using LionFire.Vos.Persisters.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class VosFrameworkHostingExtensions
{
    public static IServiceCollection AddVosArchives(this IServiceCollection services, string path)
            => services
        .Configure<VosOptions>(vo =>
        {
            vo.PrimaryRootInitializers.Add(root =>
            {
                var list = new List<IParticipant>();

                list.Add(new Participant(key: "VosArchives for " + path)
                {
                    StartAction = () =>
                    {
                        (root[path] as IVobInternals).GetOrAddVobNode<ArchivePlugin>();
                    }
                });
                return list;
            });
        })
                //.TryAddEnumerableSingleton<IVosPlugin >()
                ;
}


