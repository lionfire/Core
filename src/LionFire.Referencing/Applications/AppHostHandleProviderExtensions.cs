using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Applications.Hosting
{
    public static class AppHostHandleProviderExtensions
    {
        public static IAppHost AddHandleProvider(this IAppHost app) => app.AddSingleton<IHandleProviderService, HandleProviderService>();
    }
}
