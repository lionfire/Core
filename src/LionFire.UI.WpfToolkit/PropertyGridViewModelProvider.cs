namespace LionFire.UI.Wpf.WpfToolkit
{
    public class PropertyGridViewModelProvider : IViewModelProvider
    {
        public T ProvideViewModelFor<T>(object model, object context = null)
        {
            var rh = model as IReadHandle<object>;
            if (rh != null) model = rh.Object ?? rh;
            return (T)(object)new PropertyGridViewModel { Model = model };
        }
    }
}
