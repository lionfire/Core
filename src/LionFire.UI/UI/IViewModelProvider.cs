namespace LionFire.UI
{

    public interface IViewModelProvider
    {
        //TValue ProvideViewModelFor<TModel, TValue>(TModel model, object context = null);
        object ProvideViewModelFor(object model, object context = null);

    }
}
