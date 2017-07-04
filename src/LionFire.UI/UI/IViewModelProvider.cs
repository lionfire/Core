namespace LionFire.UI
{

    public interface IViewModelProvider
    {
        T ProvideViewModelFor<T>(object model, object context = null);
    }
}
