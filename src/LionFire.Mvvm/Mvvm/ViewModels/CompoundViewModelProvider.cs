using LionFire.Mvvm;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Types;

public class CompoundViewModelProvider : IViewModelProvider
{
    #region Dependencies

    IViewModelProvider ConstructorViewModelProvider;
    IViewModelProvider RegistryViewModelProvider;

    public IEnumerable<IViewModelProvider> ViewModelProviders
    {
        get
        {
            yield return RegistryViewModelProvider;
            yield return ConstructorViewModelProvider;
        }
    }

    #endregion

    #region Lifecycle

    public CompoundViewModelProvider(IServiceProvider serviceProvider)
    {
        ConstructorViewModelProvider = ActivatorUtilities.CreateInstance<ConstructorViewModelProvider>(serviceProvider);
        RegistryViewModelProvider = ActivatorUtilities.CreateInstance<RegistryViewModelProvider>(serviceProvider);
    }

    #endregion

    #region Pass-thru

    public TViewModel? TryActivate<TViewModel, TModel>(TModel model, Action<TViewModel>? initializer = null, params object[]? constructorParameters)
    {
        foreach (var p in ViewModelProviders)
        {
            var result = p.TryActivate<TViewModel, TModel>(model, initializer, constructorParameters);
            if (result != null) return result;
        }
        return default;
    }

    public object? TryActivate<TModel>(TModel model, Action<object>? initializer = null, params object[]? constructorParameters)
    {
        foreach (var p in ViewModelProviders)
        {
            var result = p.TryActivate<TModel>(model, initializer, constructorParameters);
            if (result != null) return result;
        }
        return null;
    }

    public bool CanTransform<T1, T2>()
    {
        foreach (var p in ViewModelProviders)
        {
            if (p.CanTransform<T1, T2>()) return true;
        }
        return false;
    }

    #endregion
}
