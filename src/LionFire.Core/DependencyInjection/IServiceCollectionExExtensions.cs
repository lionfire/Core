namespace LionFire.DependencyInjection; 

public static class IServiceCollectionExExtensions
{
    public static IServiceCollectionEx AddUnboundGenericTransient(this IServiceCollectionEx services, Type serviceType, Type instanceType)
    {
        services.Add(new ServiceDescriptorEx(serviceType, instanceType));
        return services;
    }

    public static IServiceCollectionEx AddUnboundGenericTransient(this IServiceCollectionEx services, Type serviceType, Func<Type, object[], object> factory)
    {
        services.Add(new ServiceDescriptorEx(serviceType, factory));
        return services;
    }
}
