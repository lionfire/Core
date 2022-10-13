using LionFire.Mvvm;
using LionFire.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LionFire.Types;

public class ViewModelProvider : IViewModelProvider
{

    #region Dependencies

    ///// <summary>
    ///// Optional
    ///// </summary>
    //public TypeScanService? TypeScanService { get; }
    public IServiceProvider ServiceProvider { get; }
    public ViewModelTypeRegistry ViewModelTypeRegistry { get; }

    #endregion

    #region Lifecycle

    public ViewModelProvider(IServiceProvider serviceProvider, ViewModelTypeRegistry viewModelTypeRegistry)
    {
        //TypeScanService = serviceProvider.GetService<TypeScanService>();
        ServiceProvider = serviceProvider;
        ViewModelTypeRegistry = viewModelTypeRegistry;
    }

    #endregion





    public TViewModel Activate<TViewModel, TModel>(TModel input, params object[] constructorParameters)
    {
        ArgumentNullException.ThrowIfNull(input);

        // TODO: if registryName is null, use a global registry or all registries?

        var inputType = input.GetType();
        var viewModelType = ViewModelTypeRegistry.GetViewModelType(typeof(TModel));

        var actualConstructorParameters = InjectionReflectionCache<TViewModel, TModel>.IncludeInputInConstructorParameters ? (constructorParameters == null ? new object[] { input } : constructorParameters.ToList().Concat(new object[] { input! })) : constructorParameters;

        // OPTIMIZE maybe: custom implementation of Activate.
        // Fall back from ServiceProvider Activator to plain Activator
        // Remember if instanceType isn't registered 
        var result = viewModelType.Activate<TViewModel>(actualConstructorParameters?.ToArray(), ServiceProvider);

        if (!InjectionReflectionCache<TViewModel, TModel>.IncludeInputInConstructorParameters)
        {
            if (InjectionReflectionCache<TViewModel, TModel>.InjectProperty != null)
            {
                InjectionReflectionCache<TViewModel, TModel>.InjectProperty.SetValue(result, input);
            }
            else if (InjectionReflectionCache<TViewModel, TModel>.InjectField != null)
            {
                InjectionReflectionCache<TViewModel, TModel>.InjectField.SetValue(result, input);
            }
            else
            {
                throw new Exception($"{typeof(TViewModel).FullName} must have a constructor parameter of type ${typeof(TModel)}, or else a settable Property or Field of that type.");
            }
        }

        return result;
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
