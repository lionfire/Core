using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class MudBlazorComponentsHostingX
{
    public static IServiceCollection AddMudBlazorComponents(this IServiceCollection services)
    {
        return services
            .AddUIComponents()
            ;
    }   
}
