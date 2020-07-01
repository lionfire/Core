#if WINDOWS
using System.Windows.Controls;
using Dock = System.Windows.Controls.Dock;
#elif NOESIS
using Dock = Noesis.Dock;
#endif

namespace LionFire.Valor.Interface
{
#if !WINDOWS && !NOESIS
    public enum Dock // COMPATIBILITY
    {
        Left,
        Top,
        Right,
        Bottom
    }

#endif

    public interface IDockableUI
    {
        Dock Dock { get; set; }

    }
}
