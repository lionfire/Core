using LionFire.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Splat.ModeDetection;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace LionFire.Types;

public class ConstructorViewModelProvider : IViewModelProvider
{
    public IServiceProvider ServiceProvider { get; }

    public ConstructorViewModelProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public TViewModel? TryActivate<TViewModel, TModel>(TModel model, Action<TViewModel>? initializer = null, params object[]? constructorParameters)
    {
        try
        {
            return ActivatorUtilities.CreateInstance<TViewModel>(ServiceProvider, typeof(TViewModel), (constructorParameters ?? Array.Empty<object>()).Concat(new object?[] { model }).ToArray());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception {nameof(ConstructorViewModelProvider.TryActivate)} failed ActivatorUtilities.CreateInstance for type {typeof(TViewModel).FullName}:");
            Debug.WriteLine(ex);
        }
        return default;
    }

    /// <summary>
    /// Not supported since we don't know the ViewModel type
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    /// <param name="constructorParameters"></param>
    /// <returns></returns>
    public object? TryActivate<TModel>(TModel model, Action<object>? initializer = null, params object[]? constructorParameters) => null;

    public bool CanTransform<TModel, TViewModel>()
    {
        return canTransformCache.GetOrAdd((typeof(TModel), typeof(TViewModel)), _ => _CanTransform<TModel, TViewModel>());
    }
    private bool _CanTransform<TModel, TViewModel>()
    {
        try
        {
            var model = ActivatorUtilities.CreateInstance<TModel>(ServiceProvider);
            var result = TryActivate<TViewModel, TModel>(model);
            return result != null;
        }
        catch { }
        return false;
    }

    private ConcurrentDictionary<(Type, Type), bool> canTransformCache = new();
}
