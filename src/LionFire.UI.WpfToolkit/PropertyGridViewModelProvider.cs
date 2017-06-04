namespace LionFire.UI.Wpf.WpfToolkit
{
    public class PropertyGridViewModelProvider : IViewModelProvider
    {
        public object ProvideViewModel(object model)
        {
            var rh = model as IReadHandle<object>;
            if (rh != null) model = rh.Object ?? rh;
            return new PropertyGridViewModel { Model = model };
        }
    }
}
