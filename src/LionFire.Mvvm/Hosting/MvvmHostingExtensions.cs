using LionFire.Mvvm;
using LionFire.Types;
using LionFire.Types.Scanning;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LionFire.Hosting;

public static class MvvmHostingExtensions
{
    public static IServiceCollection AddMvvm(this IServiceCollection services, params Assembly[] viewModelAssemblies)
    {
        return services
            .AddSingleton<ViewModelTypeRegistry>()
            .AddHostedService(s=>s.GetRequiredService<ViewModelTypeRegistry>())
            .If(viewModelAssemblies.Length > 0, s => s.Configure<ViewModelConfiguration>(c => c.TypeScanOptions.AssemblyWhitelist = viewModelAssemblies))
            .AddSingleton<IViewModelProvider, ViewModelProvider>()

            //.AddHostedService<TypeScanService>() // TODO: move this line to LionFire.Core/Hosting extensions?
            //.Configure<TypeScanOptions>(o =>
            //{
            //    o.ScanJobs.Add(new ScanJobParameters
            //    {
            //        Name = MvvmConstants.ScanJobName,
            //        FindType = typeof(IViewModel<>),
            //        Key = t => t.GetGenericArguments(1),
            //    });
            //})
            ;
    }
}
