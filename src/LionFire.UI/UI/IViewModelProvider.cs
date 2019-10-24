namespace LionFire.UI
{

    public interface IViewModelProvider
    {
        //T ProvideViewModelFor<TModel, T>(TModel model, object context = null);
        object ProvideViewModelFor(object model, object context = null);

    }
}
