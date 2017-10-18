using LionFire.Applications.Hosting;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Hosting
{
    public static class FsObjectBusAppHostExtensions
    {
        public static IAppHost UseFsBus(this IAppHost app)
        {
            return app.AddInit(_ =>
            {
                // TODO TOUNGLOBAL: Remove global
                SchemeBroker.Instance.Register(FsOBaseProvider.Instance); // HARDCODE HARDCONF
                return true;
            });
        }
    }
}
