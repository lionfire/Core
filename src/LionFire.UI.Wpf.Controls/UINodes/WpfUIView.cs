#if NOESIS
using Noesis;
#else
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.VisualStyles;
#endif

namespace LionFire.UI.Entities.Wpf
{
    // UNUSED - Not sure there's any point to this class
    public class WpfUIView<T> : UIView<T>
        where T : UIElement
    {
    }
}
