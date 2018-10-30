using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.Vos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Hosting
{
    public static class VosAppHostExtensions
    {
        public static IAppHost AddVos(this IAppHost app)=> app.TryAddEnumerableSingleton<IOBus, VosOBus>();
        
    }
}
