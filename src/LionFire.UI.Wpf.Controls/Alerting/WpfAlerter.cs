using System;
using LionFire.Alerting;
//using AppUpdate;
//using AppUpdate.Common;

namespace LionFire.Shell
{
    public class WpfAlerter : IAlerter
    {

        public WpfAlerter()
        {

        }

        #region IsAlertOpen

        public bool IsAlertOpen
        {
            get => isAlertOpen;
            set
            {
                if (isAlertOpen == value) return;
                isAlertOpen = value;

                var ev = IsAlertOpenChanged;
                if (ev != null) ev();
            }
        }


        private bool isAlertOpen;

        public event Action IsAlertOpenChanged;

        #endregion

        public virtual void Alert(Alert alert)
        {
            throw new NotImplementedException("TOPORT");
#if TOPORT
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(new Action(() => Alert(alert))); return; }
            var layer = MainPresenter.TopControl;

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
#endif
        }

    }
}
