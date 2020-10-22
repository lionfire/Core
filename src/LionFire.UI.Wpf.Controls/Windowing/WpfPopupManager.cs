using LionFire.Shell;
using LionFire.UI;
using System;
using System.Windows;

namespace LionFire.Hosting
{
    public interface IPopupManager
    {
        void LaunchPopupWindow(ViewReference ui);
    }

    public class LionFireWpfPopupManager : IPopupManager
    {
        public void LaunchPopupWindow(ViewReference ui)
        {
            //        var fe = (FrameworkElement)Activator.CreateInstance(type);

            //        var scp = new ShellContentPresenter(this);
            //        scp.Content = fe;

            //        scp.MinWidth = 20;
            //        scp.Width = double.NaN;
            //        scp.Height = double.NaN;
            //        scp.ShellWindow.SizeToContent = SizeToContent.WidthAndHeight;
            //        //scp.ShellWindow.WindowStyle = WindowStyle.None;

            //        scp.Show();
        }
    }
}
