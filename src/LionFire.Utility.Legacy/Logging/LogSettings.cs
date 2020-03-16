#if OLD  // Migrate to Microsoft.Extensions.Configuration for logging config
using LionFire.Persistence;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Logging
{
    public class LogSettings : INotifyOnRetrieve
    {
        #region (Static) Instance from Vos

        public static bool HasInstance => VLogSettings != null && VLogSettings.HasObject;
        public static LogSettings Instance => VLogSettings.TryGetOrCreate();

        public static IReadWriteHandle<LogSettings> VLogSettings { get { return !V.HasActiveData ? null : V.ActiveData["AppSettings/LogSettings"].ToHandle<LogSettings>(); } }

        #endregion

        #region LogLevels

        public Dictionary<string, LogLevel> LogLevels { get; set; } = new Dictionary<string, LogLevel>();

        #endregion

        #region Disabled

        public HashSet<string> Disabled { get; set; } = new HashSet<string>();

        #endregion

        #region Event Handling

        public void OnRetrieved() => LoadLogSettings();

        //public void OnRetrieved(IVobHandle vh) => LoadLogSettings();

        #endregion

        #region (Public) Methods

        public void LoadLogSettings()
        {
                foreach(var log in Log.Loggers)
                {
                    ApplyLogSettings(log);
                }
        }

        #endregion


        public void ApplyLogSettings(ILogger log)
        {
            if (log == null) return;

            if (Disabled != null && Disabled.Contains(log.Name))
            {
                log.LogLevel = LogLevel.Disabled;
                return;
            }

            if (LogLevels != null)
            {
                LogLevel level;

                if (LogLevels.TryGetValue(log.Name, out level))
                {
                    log.LogLevel = level;
                    return;
                }
            }

            if (log.LogLevel == LogLevel.Disabled)
            {
                // ENH: Store previous log level?  Use a bool flag to disable?
                log.LogLevel = LogLevel.All;
            }
        }
    }
}
#endif