using System;
using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using DependencyContext = LionFire.Dependencies.DependencyContext;

namespace LionFire.Hosting;

/// <summary>
/// Grabs the ServiceProvider from the wrapped IServiceProviderFactory and stores it in DependencyContext.Current.ServiceProvider
/// </summary>
public class CaptureServiceProviderIntoContextAndWrapFactory : IServiceProviderFactory<IServiceCollection>
{
    public bool MultiHostEnvironment { get; }
    public bool UseAsGuaranteedSingletonProvider { get; }

    private IServiceProviderFactory<IServiceCollection> child;

    /// <param name="child"></param>
    /// <param name="allowMultiple">If true, assign to DependencyContext.Current.ServiceProvider.  
    /// If false, assign to DependencyContext.Current.SingleRootServiceProvider.</param>
    public CaptureServiceProviderIntoContextAndWrapFactory(IServiceProviderFactory<IServiceCollection> child, bool? multiHostEnvironment = true, bool useAsGuaranteedSingletonProvider = false)
    {
        this.child = child;
        MultiHostEnvironment = multiHostEnvironment ??= LionFireEnvironment.IsMultiApplicationEnvironment;
        UseAsGuaranteedSingletonProvider = useAsGuaranteedSingletonProvider;
    }

    protected void OnInitDependencyContext(DependencyContext dependencyContext, IServiceProvider serviceProvider)
    {
        dependencyContext.ServiceProvider = serviceProvider;
        if (UseAsGuaranteedSingletonProvider)
        {
            dependencyContext.UseAsGuaranteedSingletonProvider();
        }
    }

    public IServiceCollection CreateBuilder(IServiceCollection services) => child.CreateBuilder(services);
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        var result = child.CreateServiceProvider(containerBuilder);

        // REVIEW
        OnInitDependencyContext(MultiHostEnvironment ? DependencyContext.AsyncLocal : DependencyContext.Global, result);

        //if (AllowMultiple)
        //{
        //    OnInitDependencyContext(DependencyContext.Current, result);

        //}
        //else
        //{
        //    DependencyContext.GlobalServiceProvider = result;
        //    if (UseAsGuaranteedSingletonProvider)
        //    {
        //        DependencyContext.GlobalServiceProvider.UseAsGuaranteedSingletonProvider();
        //    }
        //}

        return result;
    }
}
