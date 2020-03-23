using LionFire.Persistence;

namespace LionFire.UI.Wpf.WpfToolkit
{
    public class PropertyGridViewModelProvider : IViewModelProvider
    {
        public object ProvideViewModelFor(object model, object context = null)
        {
            var rh = model as IReadHandle<object>;
            if (rh != null) model = rh.Value ?? rh;
            return new PropertyGridViewModel { Model = model };
        }
    }
}
