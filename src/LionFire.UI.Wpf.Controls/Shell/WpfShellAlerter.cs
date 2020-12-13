#if OLD
using System;
using System.Windows.Controls;
using LionFire.Avalon;
//using AppUpdate;
//using AppUpdate.Common;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using LionFire.Alerting;
using LionFire.Threading;
using LionFire.Dependencies;
using System.Windows;

namespace LionFire.Shell
{
    public class WpfShellAlerter : IAlerter
    {
        public static WpfShellAlerter Instance => DependencyContext.Current.GetService<WpfShellAlerter>();

        public WpfShellAlerter(WpfShell wpfShell)
        {
            WpfShell = wpfShell;
        }

        public WpfShell WpfShell { get; }


        #region IsAlertOpen

        public bool IsAlertOpen
        {
            get { return isAlertOpen; }
            set
            {
                if (isAlertOpen == value) return;
                isAlertOpen = value;
                IsAlertOpenChanged?.Invoke();
            }
        }
        private bool isAlertOpen;

        public event Action IsAlertOpenChanged;

        #endregion

        public virtual void Alert(Alert alert)
        {
            if (!WpfShell.Dispatcher.CheckAccess()) { WpfShell.Dispatcher.BeginInvoke(new Action(() => Alert(alert))); return; }
            var layer = WpfShell.ShellPresenter.MainPresenter.TopControl as Panel;

            if (layer == null)
            {
                l.LogCritical("Failed to get adorner layer for alert: " + alert.Message + " " + alert.Exception);
                // TODO: Throw exception and use a fallback alerter
                return;
            }

            var a = new AlertAdorner() { DataContext = alert, Layer = layer };
            a.SetValue(Grid.ZIndexProperty, 99);
            layer.Children.Add(a);
            IsAlertOpen = true;
        }

        private static ILogger l = Log.Get();

    }
}
#endif