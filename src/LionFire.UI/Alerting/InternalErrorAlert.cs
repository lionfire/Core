using Microsoft.Extensions.Logging;
using System;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{
    public class InternalErrorAlert : Alert
    {
        public InternalErrorAlert()
        {
        }

        public InternalErrorAlert(string message, Exception ex = null)
            : base(message, ex)
        {
        }
        public InternalErrorAlert(string message, string detail)
            : base(message, detail)
        {
        }
        public InternalErrorAlert(string message)
            : base(message)
        {
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();
            Title = "Internal Error";
            LogLevel = LogLevel.Error;
            Flags |= AlertFlags.InternalError;
        }

    }

}
