using LionFire.Mvvm;
using LionFire.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Types;

public class RegistryViewModelProvider : IViewModelProvider
{

    #region Dependencies

    public IServiceProvider ServiceProvider { get; }
    public ViewModelTypeRegistry? ViewModelTypeRegistry { get; }

    #endregion

    #region Lifecycle

    public RegistryViewModelProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ViewModelTypeRegistry = serviceProvider.GetService<ViewModelTypeRegistry>();
    }

    #endregion

    #region IViewModelProvider

    public object? TryActivate<TModel>(TModel model, Action<object>? initializer = null, params object[]? constructorParameters)
    {
        if (ViewModelTypeRegistry == null) return null;

        var viewModelType = TryGetViewModelType<TModel>(model);

        if (viewModelType == null)
        {
            Debug.WriteLine($"No ViewModel Type is registered in ViewModelTypeRegistry for model type ({typeof(TModel).FullName}).");
        }

        return TryActivate_Common<object, TModel>(model, viewModelType, initializer, constructorParameters);
    }

    private TViewModel? TryActivate_Common<TViewModel, TModel>(TModel model, Type? viewModelType, Action<TViewModel>? initializer, object[]? constructorParameters)
    {
        if (viewModelType == null) return default;

        var actualConstructorParameters = InjectionReflectionCache<TViewModel, TModel>.IncludeInputInConstructorParameters ? (constructorParameters == null ? new object[] { model } : constructorParameters.ToList().Concat(new object[] { model! })) : constructorParameters;

        // OPTIMIZE maybe: custom implementation of TryActivate.
        // Fall back from ServiceProvider Activator to plain Activator
        // Remember if instanceType isn't registered 
        var result = viewModelType.Activate<TViewModel>(actualConstructorParameters?.ToArray(), ServiceProvider);

        if (!InjectionReflectionCache<TViewModel, TModel>.IncludeInputInConstructorParameters)
        {
            if (InjectionReflectionCache<TViewModel, TModel>.InjectProperty != null)
            {
                InjectionReflectionCache<TViewModel, TModel>.InjectProperty.SetValue(result, model);
            }
            else if (InjectionReflectionCache<TViewModel, TModel>.InjectField != null)
            {
                InjectionReflectionCache<TViewModel, TModel>.InjectField.SetValue(result, model);
            }
            else
            {
                throw new Exception($"{typeof(TViewModel).FullName} must have a constructor parameter of type ${typeof(TModel)}, or else a settable Property or Field of that type.");
            }
        }

        initializer?.Invoke(result);

        return result;
    }

    public TViewModel? TryActivate<TViewModel, TModel>(TModel model, Action<TViewModel>? initializer = null, params object[]? constructorParameters)
    {
        if (ViewModelTypeRegistry == null) return default;

        ArgumentNullException.ThrowIfNull(model);

        var viewModelType = ViewModelTypeRegistry.TryGetViewModelType(typeof(TModel));

        if (viewModelType == null)
        {
            if (!typeof(TViewModel).IsInterface && !typeof(TViewModel).IsAbstract)
            {
                viewModelType = typeof(TViewModel);
            }
            else
            {
                throw new ArgumentException($"No ViewModel Type is registered in ViewModelTypeRegistry for model type ({typeof(TModel).FullName}) and TViewModel ({typeof(TViewModel).FullName}) is not instantiable.");
            }
        }

        return TryActivate_Common<TViewModel, TModel>(model, viewModelType, initializer, constructorParameters);
    }

    #endregion

    #region Helper Methods

    public Type? TryGetViewModelType<TModel>(TModel model)
    {
        if (ViewModelTypeRegistry == null) return null;
        return ViewModelTypeRegistry.TryGetViewModelType(model.GetType())
            ?? ViewModelTypeRegistry.TryGetViewModelType(typeof(TModel));
    }
    public Type? TryGetViewModelType<TViewModel, TModel>(TModel model)
    {
        if (ViewModelTypeRegistry == null) return null;

        var result = ViewModelTypeRegistry.TryGetViewModelType(model.GetType())
            ?? ViewModelTypeRegistry.TryGetViewModelType(typeof(TModel));
        if (result != null) return result!;

        if (!typeof(TViewModel).IsInterface && typeof(TViewModel).IsAbstract)
        {
            return typeof(TViewModel);
        }

        return null;
    }

    public bool CanTransform<TModel, TViewModel>()
        => ViewModelTypeRegistry?.ViewModelsByModelType.ContainsKey(typeof(TModel)) == true;

    #endregion
}
