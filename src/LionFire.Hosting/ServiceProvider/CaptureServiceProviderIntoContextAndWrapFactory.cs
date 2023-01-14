using System;
using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

/// <summary>
/// Grabs the ServiceProvider from the wrapped IServiceProviderFactory and stores it in DependencyContext.Current.ServiceProvider
/// </summary>
public class CaptureServiceProviderIntoContextAndWrapFactory : IServiceProviderFactory<IServiceCollection>
{
    public bool AllowMultiple { get; }
    private IServiceProviderFactory<IServiceCollection> child;
    
    /// <param name="child"></param>
    /// <param name="allowMultiple">If true, assign to DependencyContext.Current.ServiceProvider.  
    /// If false, assign to DependencyContext.Current.SingleRootServiceProvider.</param>
    public CaptureServiceProviderIntoContextAndWrapFactory(IServiceProviderFactory<IServiceCollection> child, bool allowMultiple = false)
    {
        this.child = child;
        AllowMultiple = allowMultiple;
    }

    public IServiceCollection CreateBuilder(IServiceCollection services) => child.CreateBuilder(services);
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        var result = child.CreateServiceProvider(containerBuilder);

        if (AllowMultiple)
        {
            DependencyContext.Current.ServiceProvider = result;
        }
        else
        {
            DependencyContext.SingleRootServiceProvider = result;
        }

        return result;
    }
}
