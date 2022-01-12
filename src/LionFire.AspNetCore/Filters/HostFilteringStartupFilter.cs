using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;

namespace LionFire.AspNetCore
{
    // MOVE to LionFire.AspNetCore.dll

    // Based on Microsoft.AspNetCore.HostFilteringStartupFilter
    public class HostFilteringStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseHostFiltering();
                next(app);
            };
        }
    }
}