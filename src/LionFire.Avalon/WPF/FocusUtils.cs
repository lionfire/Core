using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LionFire.Avalon
{
    public static class FocusUtils
    {
        public static void FocusAncestorIfFocused(this FrameworkElement fe)
        {
            if (fe.IsFocused)
            {
                fe.FocusAncestor();
            }
        }
        public static void FocusAncestor(this FrameworkElement fe)
        {
            // Move to a parent that can take focus
            FrameworkElement parent = (FrameworkElement)fe.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(fe);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }
    }
}
