using LionFire.Dependencies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LionFire.Applications
{
    public class DevMode
    {
        #region Settings

        #region Dev Mode

        /// <summary>
        /// A special back door mode for developers to access features not normally available to users
        /// </summary>
        public static bool IsDevMode
        {
            get
            {
                if (!isDevMode.HasValue)
                {
                    isDevMode = false;

#if AllowDevTxt
                    //var appDir = ServiceLocator.Get<AppInfo>()?.AppDir;
                    //isDevMode = appDir != null && File.Exists(Path.Combine(appDir, "Dev.txt"));
#endif
                }
                return isDevMode.Value;
            }
        }
        private static bool? isDevMode;

        [Conditional("DEBUG")]
        public static void IfDevMode(Action action)
        {
            if (IsDevMode) action();
        }

#endregion

#endregion
    }
}
