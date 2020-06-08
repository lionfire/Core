namespace LionFire.Shell
{
    
    public class LionFireShellOptions
    {
        public bool MinimizeAllOnFullScreen { get; set; } = true; 
        public bool UndoMinimizeAllOnRestore { get; set; } = true;

        /// <summary>
        /// Set to true to disable default Windows TitleBar and use the custom one.
        /// </summary>
        public bool UseCustomTitleBar { get; set; } = true;

        public int DefaultWindowWidth { get; set; } = 850;
        public int DefaultWindowHeight { get; set; } = 600;

        //IsFullScreenDefault => !LionFire.Applications.LionFireApp.IsDevMode; // TODO: Different default based on DevMode
        public bool StartMaximizedToFullScreen { get; internal set; }

    }
}
