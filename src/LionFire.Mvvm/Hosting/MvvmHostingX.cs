﻿using LionFire.Mvvm;
using LionFire.Types;
using LionFire.Types.Scanning;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LionFire.Hosting;
public static class MvvmHostingX
{
    public static IServiceCollection AddMvvm(this IServiceCollection services, params Assembly[] viewModelAssemblies) => services.AddMvvm(useDefaults: true, viewModelAssemblies);
    public static IServiceCollection AddMvvm(this IServiceCollection services, bool useDefaults = true, params Assembly[] viewModelAssemblies)
    {

        return services
            .UseMicrosoftDIForReactiveUI()
            .AddSingleton<ViewModelTypeRegistry>()
            .AddInspector(useDefaults)
            .AddSummarizer(useDefaults)
            .AddHostedService(s=>s.GetRequiredService<ViewModelTypeRegistry>())
            .If(viewModelAssemblies.Length > 0, s => s.Configure<ViewModelConfiguration>(c => c.TypeScanOptions.AssemblyWhitelist = viewModelAssemblies))
            .AddSingleton<IViewModelProvider, CompoundViewModelProvider>()

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
