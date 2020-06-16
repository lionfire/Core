using System;

namespace LionFire.Applications
{
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

