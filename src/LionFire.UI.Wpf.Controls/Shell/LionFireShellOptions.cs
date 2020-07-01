using System.Diagnostics;

namespace LionFire.Shell
{
    
    public class LionFireShellOptions
    {
        /// <summary>
        /// If true, Shell will invoke its own StartAsync method after IHostApplicationLifetime.ApplicationStarted fires.
        /// </summary>
        public bool AutoStart { get; set; } = false;

        public bool MinimizeAllOnFullScreen { get; set; } = true; 
        public bool UndoMinimizeAllOnRestore { get; set; } = true;

        /// <summary>
        /// Set to true to disable default Windows TitleBar and use the custom one.
        /// </summary>
        public bool UseCustomTitleBar { get; set; } = true;

        public int DefaultWindowWidth { get; set; } = 850;
        public int DefaultWindowHeight { get; set; } = 600;

        //public bool IsFullScreenDefault => !LionFire.Applications.LionFireApp.IsDevMode; // TODO: Different default based on DevMode


        // FUTURE: Default sizes for different modes: PC/Tablet/etc.
        //public virtual Size DefaultWindowedSize => new Size(1368, 768);

        public bool StartMaximizedToFullScreen { get; internal set; }

        public SourceLevels DataBindingSourceLevel { get; set; } = System.Diagnostics.SourceLevels.Verbose;

    }
}
