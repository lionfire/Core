using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;

namespace LionFire.AspNetCore
{

    // Based on Microsoft.AspNetCore.ForwardedHeadersStartupFilter
    public class ForwardedHeadersStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => 
            app => {
                app.UseForwardedHeaders();
                next(app);
            };
    }
}