using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{

    public interface IAlerter
    {
        bool IsAlertOpen { get; }

        //void Alert(string message, LogLevel level = LogLevel.Message, Exception ex = null, string detail = null, string title = null, AlertFlags flags = AlertFlags.None);

        void Alert(Alert alert);
    }

    public static class IAlerterExtensions
    {
        public static void Alert(this IAlerter alerter, string message, LogLevel level = LogLevel.Information, Exception ex = null, string detail = null, string title = null, AlertFlags flags = AlertFlags.None)
        {
            Alert alert = new Alert()
            {
                Message = message,
                LogLevel = level,
                Exception = ex,
                Detail = detail,
                Title = title,
                Flags = flags,
            };
            alerter.Alert(alert);
        }
    }

}
