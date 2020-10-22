using System;
using System.Windows.Controls;
using System.Windows.Threading;
using LionFire.Alerting;
using LionFire.Avalon;
using Microsoft.Extensions.Logging;

namespace LionFire.Shell
{
    public class WpfAlerter : IAlerter
    {

        public IShellConductor ShellPresenter { get; }
        public WpfDispatcherAdapter WpfDispatcherAdapter { get; }

        public Dispatcher Dispatcher => WpfDispatcherAdapter.Dispatcher;

        public WpfAlerter(WpfDispatcherAdapter wpfDispatcherAdapter, IShellConductor shellPresenter)
        {
            WpfDispatcherAdapter = wpfDispatcherAdapter;
            ShellPresenter = shellPresenter;
        }

        #region IsAlertOpen

        public bool IsAlertOpen
        {
            get => isAlertOpen;
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
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => Alert(alert))); return; }
            var layer = (Panel) ShellPresenter.MainPresenter.TopControl;

            if (layer == null)
            {
                l.LogCritical("Failed to get adorner layer for alert: " + alert.Message + " " + alert.Exception);
                // TODO: Throw exception and use a fallback alerter
                return;
            }

            var a = new AlertAdorner() { DataContext = alert, Layer = layer };
            a.SetValue(Panel.ZIndexProperty, 99);
            layer.Children.Add(a);
            IsAlertOpen = true;
        }

        private static ILogger l = Log.Get();
    }
}
