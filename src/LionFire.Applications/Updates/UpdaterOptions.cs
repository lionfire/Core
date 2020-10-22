

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Updates
{
    public class UpdaterOptions
    {
        public string ReleaseChannel = "Prealpha";

#if OLD
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

        public bool IsAutoUpdateCompiled // i.e. baked into the main executable  MOVE.  Change to 'is IUpdater service available from IServiceProvider)?
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
#endif
    }
    
}
