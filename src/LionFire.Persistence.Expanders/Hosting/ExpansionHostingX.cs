using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class ExpansionHostingX
{
    public static IServiceCollection Expansion(this IServiceCollection services)
    {
        return services
            .TryAddEnumerableSingleton<IReferenceProvider, ExpansionReferenceProvider>()
            .AddSingleton<IReadHandleProvider<IExpansionReference>, ExpansionHandleProvider>()
            .AddSingleton<IExpanderProvider, ExpanderProvider>()
            //.AddSingleton<IReadWriteHandleProvider<IExpansionReference>, ExpansionHandleProvider>()
            //.AddSingleton<IWriteHandleProvider<IExpansionReference>, ExpansionHandleProvider>()

            ;
    }
}
