using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Alerting
{
    public enum AlertFlags
    {
        None = 0,
        Modal = 0x01,
        MustAcknowledge = 0x02,
    }

    public interface IAlerter
    {
        void Alert(string message, LogLevel level = LogLevel.Message, Exception ex = null, string detail = null, string title = null, AlertFlags flags = AlertFlags.None);
    }

    public static class Alerter
    {
        public IAlerter Instance
        {
            get
            {
                return InstanceRegistrar.Instance.TryResolve(typeof(IAlerter));
            }
        }

        public static void Alert(string message, LogLevel level = LogLevel.Message, Exception ex = null, string detail = null, string title = null, AlertFlags flags = AlertFlags.None)
        {
            Instance.Alert(message: message, level: level, ex: ex, detail: detail, title: title, flags: flags);
        }
    }

    public class WpfAlerter : IAlerter
    {
        public void Notify(string message, LogLevel level = LogLevel.Message, Exception ex = null, string detail = null, string title = null)
        {
            MessageBox.Show(
                message 
                + (detail == null ? "": detail.ToString() + Environment.NewLine) 
                + (ex == null ? "":ex.ToString()), 
                title ?? (Char.IsLetter(level.ToString()[0])?level.ToString() : "Notice", MessageBoxButton.OK));

            //if (this.InterfaceController == null)
            //{
            //    LionFire.Applications.LionFireApp.Current.NotifyUser(message: message, level: level, ex: ex, detail: detail, title: title);
            //}
            //else
            //{
            //}
        }
    }
}
