#nullable enable
namespace LionFire.Mvvm;

public interface IViewModelProvider
{
    /// <summary>
    /// Can Transform from TModel to TViewModel
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <returns></returns>
    bool CanTransform<TModel, TViewModel>();

    //T ProvideViewModelFor<TModel, T>(TModel model, object context = null);
    //object ProvideViewModelFor(object model, object context = null);
    TViewModel? TryActivate<TViewModel, TModel>(TModel model, Action<TViewModel>? initializer = null, params object[]? constructorParameters);
    object? TryActivate<TModel>(TModel model, Action<object>? initializer = null, params object[]? constructorParameters);

    // ENH idea: generic VM type support
    //public TViewModel? TryActivate<TViewModelGeneric, TViewModel, TModel>(TModel model, params object[] constructorParameters)
    //where TViewModelGeneric is `1
    //{
    // Idea: generic TViewModel, but need another parameter
    //if(typeof(TViewModel).IsGenericTypeDefinition && typeof(TViewModel).GetGenericArguments().Length == 1)
    //{
    //    var realizedGenericViewModelType = typeof(TViewModel).MakeGenericType(model.GetType() /* alternative idea: typeof(TModel) */);
    //    return () ActivatorUtilities.CreateInstance(ServiceProvider, typeof(TViewModel), constructorParameters.Concat(new object?[] { model }).ToArray());
    //}
    //}
}

public static class IViewModelProviderX
{
    public static TViewModel Activate<TViewModel, TModel>(this IViewModelProvider viewModelProvider, TModel model, Action<TViewModel>? initializer = null, params object[]? constructorParameters)
    {
        var result = viewModelProvider.TryActivate(model, initializer, constructorParameters);

        return !EqualityComparer<TViewModel>.Default.Equals(result, default) ? result! : throw new Exception("Unable to activate.");
    }
}