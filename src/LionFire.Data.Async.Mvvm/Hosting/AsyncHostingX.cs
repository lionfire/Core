using LionFire.Data.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class AsyncHostingX
{
    public static IServiceCollection AddAsyncDataMvvm(this IServiceCollection services)
    {
        return services
            .AddSingleton(typeof(AsyncKeyedCollectionVM<,,>))
            .AddSingleton(typeof(AsyncKeyedVMCollectionVM<,,>))
            .AddSingleton(typeof(ObservableDataVM<,,>))
            ;
    }
}
