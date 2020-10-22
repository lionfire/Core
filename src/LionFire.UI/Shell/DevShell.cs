using LionFire.Dependencies;
using System;

namespace LionFire.Applications // MOVE to LionFire.Dev
{
    public class DevTools
    {
        public static bool IsDevMode
        {
            get => DependencyContext.Current.GetService<IDevApp>()?.IsDevMode == true;
            set
            {
                var devApp = DependencyContext.Current.GetService<IDevApp>();
                if (devApp != null) { devApp.IsDevMode = value; }
                // else no-op SILENTFAIL
            }
        }
    }

    public interface IDevApp
    {
        bool IsDevMode { get; set; }
    }
    public class DevApp : IDevApp
    {
        public bool IsDevMode { get; set; }

    }

    public interface IDevShell
    {

        bool DebugUIVisible { get; set; }
        event Action DebugUIVisibleChanged;
    }

    public class DisabledDevShell : IDevShell
    {
        public bool DebugUIVisible { get => false; set { } }

        public event Action DebugUIVisibleChanged;
    }

    public class DevShell : IDevShell
    {
        public static DevShell Instance => DependencyContext.Current.GetService<DevShell>();

        #region DebugUIVisible

        public bool DebugUIVisible
        {
            get => debugUIVisible;
            set
            {
                if (debugUIVisible == value) return;
                debugUIVisible = value;

                DebugUIVisibleChanged?.Invoke();
            }
        }
        private bool debugUIVisible;

        public event Action DebugUIVisibleChanged;

        #endregion
    }
}

