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
            .AddTransient(typeof(AsyncKeyedCollectionVM<,,>))
            .AddTransient(typeof(AsyncKeyedVMCollectionVM<,,>))
            ;
    }

    public static IServiceCollection AddReactivePersistenceMvvm(this IServiceCollection services)
        => services
            .AddTransient(typeof(ObservableDataVM<,,>))
            .AddTransient(typeof(ObservableReaderVM<,,>))
            .AddTransient(typeof(ObservableReaderItemVM<,,>))
            .AddTransient(typeof(ObservableReaderWriterVM<,,>))
            .AddTransient(typeof(ObservableReaderWriterItemVM<,,>))
            ;
}
