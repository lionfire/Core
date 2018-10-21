using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;

namespace LionFire.ObjectBus
{
    public static class AppHostObjectBusExtensions
    {
        public static IAppHost AddObjectBus(this IAppHost app) => app.AddInit(_ => OBaseSchemeBroker.Instance.RegisterAvailableProviders());
    }
}
