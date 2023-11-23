using Microsoft.Extensions.DependencyInjection;
using Oakton;

namespace LionFire.Oakton
{
    public class ServiceProviderCommandCreator : ICommandCreator
    {
        private readonly IServiceProvider? ServiceProvider;

        public object[] DefaultConstructorParameters { get; set; } = Array.Empty<object>();

        public ServiceProviderCommandCreator(IServiceProvider? serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IOaktonCommand CreateCommand(Type commandType)
        {
            if (ServiceProvider == null)
            {
                return (IOaktonCommand)Activator.CreateInstance(commandType);
            }
            else
            {
                return (IOaktonCommand)ActivatorUtilities.CreateInstance(ServiceProvider, commandType, DefaultConstructorParameters);
            }
        }

        public object CreateModel(Type modelType) => ServiceProvider?.GetRequiredService(modelType) ?? Activator.CreateInstance(modelType);
    }
}