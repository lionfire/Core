using JasperFx.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Oakton;

public class ServiceProviderCommandCreator : ICommandCreator
{
    private readonly IServiceProvider? ServiceProvider;

    public object[] DefaultConstructorParameters { get; set; } = Array.Empty<object>();

    public ServiceProviderCommandCreator(IServiceProvider? serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IJasperFxCommand CreateCommand(Type commandType)
    {
        if (ServiceProvider == null)
        {
            return (IJasperFxCommand)Activator.CreateInstance(commandType)!;
        }
        else
        {
            return (IJasperFxCommand)ActivatorUtilities.CreateInstance(ServiceProvider, commandType, DefaultConstructorParameters);
        }
    }

    public object CreateModel(Type modelType) => ServiceProvider?.GetRequiredService(modelType) ?? Activator.CreateInstance(modelType)!;
}