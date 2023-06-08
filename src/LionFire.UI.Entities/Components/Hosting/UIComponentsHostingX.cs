using LionFire.UI.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class UIComponentsHostingX
{
    public static IServiceCollection AddUIComponents(this IServiceCollection services)
    {
        return services
            .AddTransient<PropertyGridVM>()
            .AddTransient<PropertyGridRowVM>()
            .AddTransient<PropertyGridRowsVM>()            
            ;
    }
}
