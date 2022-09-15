using LionFire.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LionFire.Types;


public class ViewModelProvider
{

    ///// <summary>
    ///// Optional
    ///// </summary>
    //public TypeScanService? TypeScanService { get; }
    public IServiceProvider ServiceProvider { get; }
    public ViewModelTypeRegistry ViewModelTypeRegistry { get; }

    public ViewModelProvider(IServiceProvider serviceProvider, ViewModelTypeRegistry viewModelTypeRegistry)
    {
        //TypeScanService = serviceProvider.GetService<TypeScanService>();
        ServiceProvider = serviceProvider;
        ViewModelTypeRegistry = viewModelTypeRegistry;
    }

    public T Activate<T, TInput>(TInput input, string? registryName = null, object[]? constructorParameters = null, bool includeInputInConstructorParameters = true)
    {
        ArgumentNullException.ThrowIfNull(input);

        // TODO: if registryName is null, use a global registry or all registries?

        var inputType = input.GetType();
        var viewModelType = ViewModelTypeRegistry.GetViewModelType(inputType);

        var actualConstructorParameters = includeInputInConstructorParameters ? (constructorParameters == null ? new object[] { input } : constructorParameters.ToList().Concat(new object[] { input! })) : constructorParameters;

        // OPTIMIZE maybe: custom implementation of Activate.
        // Fall back from ServiceProvider Activator to plain Activator
        // Remember if instanceType isn't registered 
        return viewModelType.Activate<T>(actualConstructorParameters?.ToArray(), ServiceProvider);
    }
}

public static class ActivationExtensions
{
    public static T Activate<T>(this Type instanceType, object[]? constructorParameters = null, IServiceProvider? serviceProvider = null)
    {
        if (serviceProvider == null)
        {            
            return (T)Activator.CreateInstance(instanceType, constructorParameters) ?? throw new Exception("Failed to activate");
        }
        else
        {
            return (T)ActivatorUtilities.CreateInstance(serviceProvider, instanceType, constructorParameters ?? new object[] { });
        }
    }
}
