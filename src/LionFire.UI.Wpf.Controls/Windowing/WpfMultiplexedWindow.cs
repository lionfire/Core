using LionFire.Ontology;
using LionFire.Structures;
using LionFire.UI.Entities;
using System;
using System.Collections.Generic;

namespace LionFire.UI.Entities
{
    public class WpfMultiplexedWindow : UIKeyed, IMultiplexedWindow, IWindow
    {
        #region Construction

        public WpfMultiplexedWindow()
        {
            FullScreenWindow = new WpfFullScreenWindow() { Parent = this };
            WindowedWindow = new WpfWindowedWindow() { Parent = this };
        }

        #endregion

        #region Children

        public WpfFullScreenWindow FullScreenWindow { get; protected set; }
        IWindow IMultiplexedWindow.FullScreenWindow => FullScreenWindow; 
        public WpfWindowedWindow WindowedWindow { get; protected set; }

        IWindow IMultiplexedWindow.WindowedWindow => WindowedWindow;

        public IEnumerable<IWpfWindow> Children
        {
            get
            {
                if (FullScreenWindow != null) yield return FullScreenWindow;
                if (WindowedWindow != null) yield return WindowedWindow;
            }
        }

        #endregion

        #region (Public) Properties

        #region IsFullScreen

        public bool IsFullScreen
        {
            get => isFullScreen;
            set
            {
                if (isFullScreen == value) return;
                isFullScreen = value;
                OnPropertyChanged(nameof(IsFullScreen));
            }
        }
        private bool isFullScreen;

        #endregion

        #endregion

        #region Derived

        public IWpfWindow CurrentWindow => IsFullScreen ? (IWpfWindow)FullScreenWindow : WindowedWindow;
        IWindow IMultiplexedWindow.CurrentWindow => CurrentWindow;

        public object View
        {
            get => CurrentWindow.View;
            set
            {
                if (value == FullScreenWindow.View) { IsFullScreen = true; }
                else if (value == WindowedWindow.View) { IsFullScreen = false; }
                else { throw new ArgumentException($"View can only be FullScreenWindow.View or WindowedWindow.View"); }
            }
        }

        #endregion

        #region IWindow

        public bool IsMinimized => CurrentWindow.View.WindowState == System.Windows.WindowState.Minimized;

        public void Restore()
        {
            if(IsMinimized) { CurrentWindow.Restore(); return; }
            else if (IsFullScreen) { IsFullScreen = false; }
        }

        public void Minimize() => CurrentWindow.View.WindowState = System.Windows.WindowState.Minimized;

        public void Maximize()
        {
            IsFullScreen = true;
            FullScreenWindow.View.WindowState = System.Windows.WindowState.Maximized;
        }

        #endregion

        #region Pass-through


        #region Topmost

        public bool Topmost
        {
            get => CurrentWindow.View.Topmost;
            set => CurrentWindow.View.Topmost = value;
        }

        #endregion

        #endregion
    }
}
