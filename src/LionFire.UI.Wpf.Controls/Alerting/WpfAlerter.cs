using System;
using System.Windows.Controls;
using System.Windows.Threading;
using LionFire.Alerting;
using LionFire.Avalon;
using LionFire.UI.Entities;
using Microsoft.Extensions.Logging;
using LionFire.UI.Entities.Conventions;
using LionFire.Collections;

namespace LionFire.UI.Entities.Wpf
{
    public class WpfAlerter : IAlerter
    {

        public IUIRoot UIRoot { get; }
        public WpfDispatcherAdapter WpfDispatcherAdapter { get; }

        public Dispatcher Dispatcher => WpfDispatcherAdapter.Dispatcher;

        public WpfAlerter(WpfDispatcherAdapter wpfDispatcherAdapter, IUIRoot uiRoot)
        {
            WpfDispatcherAdapter = wpfDispatcherAdapter;
            UIRoot = uiRoot;
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

            var layer = (Panel) ((IHierarchyOfKeyed<IUIKeyed>)UIRoot.MainWindow()) .QuerySubPath(UIConventions.Keys.Layers, UIConventions.Keys.TopLayer);

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
