using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Hosting
{
    public static class VosAppHostExtensions
    {
        public static IAppHost UseVos(this IAppHost app)
        {
            return app.AddInit(_ =>
            {
                // TODO TOUNGLOBAL: Remove global
                SchemeBroker.Instance.Register(VosOBaseProvider.Instance);
                return true;
            });
        }
    }
}
