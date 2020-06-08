using System;

namespace LionFire.Applications
{
    public class DevShell
    {
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

