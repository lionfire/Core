#if NAppUpdater
using AppUpdate;
using AppUpdate.Tasks;
using AppUpdate.Common;
#endif

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Updates
{
    public class AppUpdateService
    {
        public bool FirstRunWithNewVersion;

        public int UpdatesAvailable
        {
            get
            {
#if NAppUpdater
                return UpdateManager.Instance.UpdatesAvailable;
#else
                return 0;
#endif
            }
        }

#if TOPORT
        #region AutoUpdate

        #region Configuration

        // TODO: Get from user config, allow user to change somehow
        public string CurrentReleaseChannel = "Prealpha";

        public static bool AutoUpdateAsync = false;
        public static bool AutoUpdateAlwaysRestart = true;

        /// <summary>
        /// Override this to allow auto updates
        /// </summary>
        public virtual bool AllowAutoUpdates
        {
            get { return false; }
        }

        public bool IsAutoUpdateEnabled
        {
            get
            {
                if (!AllowAutoUpdates) { return false; }
                if (!isAutoUpdateEnabled.HasValue)
                {
                    if (IsDevMode)
                    {
                        l.Info("[AUTOUPDATE] skipped due to DevMode");
                        isAutoUpdateEnabled = false;
                    }
                    else
                    {
                        isAutoUpdateEnabled = IsAutoUpdateAvailable;
                    }
                }
                return isAutoUpdateEnabled.Value;
            }
        }
        private bool? isAutoUpdateEnabled;

        public bool IsAutoUpdateCompiled
        {
            get
            {
#if NAppUpdater
                return true;
#else
                return false;
#endif
            }
        }
        public bool IsAutoUpdateAvailable
        {
            get
            {
                if (!UpdateFeedUrls.Any()) return false;
#if NAppUpdater
                return true;
#else
                return false;
#endif
            }
        }

        #region URLs

        //public string UpdateUrl = "http://lionfire.ca/files/test/feed.xml";

        //https://files.lionfire.ca/files/V/ForCom/PC/Prealpha/feed.xml

        public string[] UpdateHosts = new string[]
        {
            //"http://lionfire.ca/files/V/",
            "https://lionfire.ca/files/V/",
            //"http://files.lionfire.ca/files/V/",
        };

        #endregion

        #endregion

        #endregion

#endif
        //private static ILogger lAutoUpdate { get; } = Log.Get("LionFire.Applications.LionFireApp.AutoUpdater");

    }
}
