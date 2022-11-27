namespace LionFire.DependencyInjection;

public class ServiceDescriptorEx
{
    public ServiceDescriptorEx(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    public ServiceDescriptorEx(Type serviceType, Func<Type, object[], object> factory)
    {
        ServiceType = serviceType;
        Factory = factory;
    }

    public Type ServiceType { get; }
    public Type? ImplementationType { get; }

    public Func<Type, object[], object>? Factory { get; }

}
