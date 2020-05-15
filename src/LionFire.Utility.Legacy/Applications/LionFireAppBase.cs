using LionFire.Dependencies;
using LionFire.MultiTyping;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Shell;
using LionFire.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Applications
{

    public class LionFireAppBase : MultiType
    {
        // REFACTOR - move this to a decorator

        #region StartTime

        public DateTime StartTime
        {
            get { return startTime; }
            protected set
            {
                if (startTime == value) return;
                if (startTime != default(DateTime)) throw new AlreadySetException();
                startTime = value;
            }
        } private DateTime startTime;

        #endregion



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

        #region Settings

        public virtual IEnumerable<IReadWriteHandle> SettingsObjects { get { yield break; } }

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
                    var appDir = ServiceLocator.Get<AppInfo>()?.AppDir;
                    isDevMode = appDir != null && File.Exists(Path.Combine(appDir, "Dev.txt"));
                    //l.Info("Dev mode: " + isDevMode.Value);
                }
                return isDevMode.Value;
            }
        } private static bool? isDevMode;

        [Conditional("DEBUG")]
        public static void IfDevMode(Action action)
        {
            if (IsDevMode) action();
        }

        #endregion


        #endregion

        #region Shell

        /// <summary>
        /// Be sure to set this in the constructor or shortly after. (Unless the app is headless)
        /// </summary>
        public ILionFireShell Shell
        {
            get => shell;
            set
            {
                if (shell == value) return;
                if (shell != null) throw new AlreadySetException();
                shell = value;
            }
        }
        private ILionFireShell shell;

        #endregion

        #region State

        public bool FirstRunWithNewVersion;

        protected virtual void OnStarting()
        {
        }

        protected virtual void OnStarted()
        {
        }

        public virtual bool OnClosing()
        {
            return true;
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }
    
}
