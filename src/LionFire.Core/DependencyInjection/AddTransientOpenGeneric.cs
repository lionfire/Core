using Microsoft.Extensions.DependencyInjection;

namespace LionFire.DependencyInjection;

public static class AddTransientUnboundGeneric
{
    public static IServiceCollection AddUnboundGenericTransient(this IServiceCollection services, Type interfaceType, Type implementationType)
        => services.Configure<ServiceCollectionEx>(servicesEx =>
            servicesEx.AddUnboundGenericTransient(interfaceType, implementationType)
        );
    public static IServiceCollection AddUnboundGenericTransient(this IServiceCollection services, Type interfaceType, Func<Type, object[], object>  factory)
        => services.Configure<ServiceCollectionEx>(servicesEx =>
            servicesEx.AddUnboundGenericTransient(interfaceType, factory)
        );

    

    //new ServiceDescriptorEx
    //var x = (IFactory<>)Activator.CreateInstance(typeof(TransientGenericFactory<>).MakeGenericType(interfaceType, implementationType)); 

    //return services.AddSingleton<IFactory<TInterface>>(() => x);

}
