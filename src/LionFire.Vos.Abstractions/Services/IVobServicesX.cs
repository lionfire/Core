﻿using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Vos.Services;

public static class IVobServicesX // Another version exists in LionFire.Vos.dll
{

    public static IServiceProvider ServiceProvider(this IVob vob)
        => vob.GetNextRequired<IServiceProvider>();
    public static IServiceCollection ServiceCollection(this IVob vob)
        => vob.GetNextRequired<IServiceCollection>();


    public static T GetService<T>(this IVob vob) where T : class
        => (T)vob.Acquire<IServiceProvider>()?.GetService(typeof(T));

    public static T GetRequiredService<T>(this IVob vob)
    {
        var serviceProvider = vob.GetNextRequired<IServiceProvider>();
        var result = (T)serviceProvider.GetService(typeof(T));
        if (result == null) throw new HasUnresolvedDependenciesException($"{typeof(T).FullName} service not available for vob '{vob}'");
        return result;
    }
}
