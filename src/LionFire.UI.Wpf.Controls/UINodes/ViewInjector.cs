#if NOESIS
using Noesis;
#else
#endif

namespace LionFire.UI.Wpf
{
    public class ViewInjector
    {
        public IViewLocator ViewLocator { get; }

        public ViewInjector(IViewLocator viewLocator)
        {
            ViewLocator = viewLocator;
        }


        public void Show(IUINode node, object viewModel)
        {

        }
    }
}
