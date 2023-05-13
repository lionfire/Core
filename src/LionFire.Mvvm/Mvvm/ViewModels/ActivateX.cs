using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Types;

public static class ActivateX
{
    public static T Activate<T>(this Type instanceType, object[]? constructorParameters = null, IServiceProvider? serviceProvider = null)
    {
        if (serviceProvider == null)
        {
            return (T?)Activator.CreateInstance(instanceType, constructorParameters) ?? throw new Exception("Failed to activate");
        }
        else
        {
            return (T)ActivatorUtilities.CreateInstance(serviceProvider, instanceType, constructorParameters ?? Array.Empty<object>());
        }
    }
}
