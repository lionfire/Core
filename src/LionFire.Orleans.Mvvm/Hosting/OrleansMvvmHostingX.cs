using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LionFire.Orleans_.Mvvm;
using LionFire.Mvvm;

namespace LionFire.Hosting;

public static class OrleansMvvmHostingX
{
    public static IServiceCollection AddOrleansMvvm(this IServiceCollection services)
    {
        return services
            .AddSingleton(typeof(GrainPageVM<,>));
    }
}
