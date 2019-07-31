using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
#if NET35
using ManualResetEventSlim = System.Threading.ManualResetEvent; // REVIEW
#endif

namespace LionFire.Alerting
{
    public class Alert
    {
        protected virtual void SetDefaults()
        {
        }

        #region Construction
        
        public Alert() { SetDefaults(); }

        public Alert(string message, System.Exception ex)
        {
            this.Message = message;
            this.Exception = ex;
            LogLevel = LogLevel.Error;

            SetDefaults();
        }

        public Alert(string message, string detail)
        {
            this.Message = message;
            this.Detail = detail;
            LogLevel = LogLevel.Error;

            SetDefaults();
        }

        public Alert(string message)
        {
            this.Message = message;
            LogLevel = LogLevel.Error;

            SetDefaults();
        }

        #endregion

        #region Properties

        public string Message { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public Exception Exception { get; set; }
        public string ExceptionString { get { return Exception == null ? "" : Exception.ToString(); } }
        public LogLevel LogLevel { get; set; }
        public AlertFlags Flags { get; set; }
        public IEnumerable<AlertButton> Buttons { get; set; }

        public DialogResult DialogResult { get; set; }

        #region Derived

        public bool HasTitle  { get { return !string.IsNullOrWhiteSpace(Title); } }
        public bool HasDetail { get { return !string.IsNullOrWhiteSpace(Detail); } }
        public bool HasMessage { get { return !string.IsNullOrWhiteSpace(Message); } }
        public bool ShowTextEntry { get { return Flags.HasFlag(AlertFlags.TextEntry); } }

        #endregion

        #endregion

    }

}
