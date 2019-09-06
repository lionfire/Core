using LionFire.Persistence;

namespace LionFire.UI.Wpf.WpfToolkit
{
    public class PropertyGridViewModelProvider : IViewModelProvider
    {
        public object ProvideViewModelFor(object model, object context = null)
        {
            var rh = model as RH<object>;
            if (rh != null) model = rh.Object ?? rh;
            return new PropertyGridViewModel { Model = model };
        }
    }
}
