using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;

namespace LionFire.Applications
{
    public static class IAppHostExtensions
    {
        public static T GetService<T>(this IAppHost app)
        {
            if (app.ServiceProvider == null)
            {
                throw new InvalidOperationException($"{nameof(app.ServiceProvider)} is not available");
            }

            return (T)app.ServiceProvider.GetService(typeof(T));
        }
    }
}
