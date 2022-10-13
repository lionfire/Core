#nullable enable
namespace LionFire.Mvvm;

public interface IViewModelProvider
{
    //T ProvideViewModelFor<TModel, T>(TModel model, object context = null);
    //object ProvideViewModelFor(object model, object context = null);
    TViewModel Activate<TViewModel, TModel>(TModel model, params object[] constructorParameters);

}
