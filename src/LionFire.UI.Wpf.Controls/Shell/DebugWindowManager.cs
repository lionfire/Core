using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace LionFire.Shell
{
    // TOPORT - figure out how to initialize this
    public class DebugWindowManager
    {
        protected virtual Func<FrameworkElement> DebugWindowContent { get { return () => null; } }

        public Window DebugWindow { get { return debugWindow; } }
        protected Window debugWindow;

        public bool IsDebugWindowVisible
        {
            get { return debugWindow != null && debugWindow.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    var content = DebugWindowContent();
                    if (content == null)
                    {
                        l.Warn("IsDebugWindowVisible set to true, but LionFireShell.DebugWindowContent returned null.  (Override this to specify content.)");
                        return;
                    }

                    if (debugWindow == null)
                    {
                        debugWindow = new Window();
                        //debugWindow.Content = new ValorDebugPanel();
                        debugWindow.Content = content;
                        debugWindow.Width = 500;
                        debugWindow.Height = 500;
                        //debugWindow.SizeToContent = SizeToContent.WidthAndHeight;
                    }
                    debugWindow.Show();
                }
                else
                {
                    if (debugWindow != null)
                    {
                        debugWindow.Close();
                        debugWindow = null;
                    }
                }
            }
        }

        private static ILogger l = Log.Get();

    }
}
