using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LionFire.Avalon.WinForms;

namespace LionFire.Avalon
{
    public static class WindowUtils
    {
        public static void SetRect(this Window w, Rect rect)
        {
            w.Top = rect.Top;
            w.Left = rect.Left;
            w.Width = rect.Width;
            w.Height = rect.Height;
        }
        public static Rect GetRect(this Window w )
        {
            Rect rect = new Rect(w.Left, w.Top, w.Width, w.Height);
            return rect;
        }

        public static void ConstrainWindowToScreen(this Window w, double maxSizePercentage = 0.85)
        {
            var screen = WpfScreen.GetScreenFrom(w);
            var screenArea = screen.WorkingArea;

            Rect result = ConstrainRectToRect(w.GetRect(), screenArea, maxSizePercentage);

            w.SetRect( result);
            
        }
        public static Rect ConstrainRectToRect(Rect windowRect, Rect screenArea, double maxSizePercentage = 0.85, Thickness margin = default(Thickness))
        {
            if (margin != default(Thickness)) throw new NotImplementedException("margin");

            if (windowRect.Width > (screenArea.Width * maxSizePercentage) )
            {
                windowRect.Width = screenArea.Width * maxSizePercentage;
            }
            if (windowRect.Height > (screenArea.Height * maxSizePercentage))
            {
                windowRect.Height = screenArea.Height * maxSizePercentage;
            }

            double minTop = ((1 - maxSizePercentage) * screenArea.Height);
            double minLeft = ((1 - maxSizePercentage) * screenArea.Width);

            double maxBottom = screenArea.Top + maxSizePercentage * screenArea.Height;
            double maxRight = screenArea.Left + maxSizePercentage * screenArea.Width;

            if (windowRect.Top < minTop) { windowRect.Y = minTop; }

            if (windowRect.Left < minLeft) windowRect.X = minLeft;

            if (windowRect.Bottom > maxBottom)
            {
                windowRect.Y -= windowRect.Bottom - maxBottom;
            }
            if (windowRect.Right > maxRight)
            {
                windowRect.X -= windowRect.Right - maxRight;
            }
            return windowRect;
        }
    }
}
