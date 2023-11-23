using Microsoft.Extensions.DependencyInjection;
using LionFire.ExtensionMethods;

namespace LionFire.DependencyInjection;

public static class ActivatorUtilitiesEx
{
    public static TResult CreateInstance<TResult>(this IServiceProvider serviceProvider, params object[] parameters)
        => serviceProvider.GetRequiredService<TransientGenericFactories>().Create<TResult>(parameters);

    public static bool TryCreateInstance<TResult>(this IServiceProvider serviceProvider, out TResult instance, params object[] parameters)
    {
        var f = serviceProvider.GetService<TransientGenericFactories>();
        if (f == null) { instance = default; return false; }
        return f.TryCreate(out instance, parameters);
    }

    //public static bool TryCreateInstance<TResult>(this IServiceProvider serviceProvider, out TResult instance, params object[] parameters)
    //    => TryCreateInstance<TResult>(serviceProvider, typeof(TResult), out instance, parameters);

}