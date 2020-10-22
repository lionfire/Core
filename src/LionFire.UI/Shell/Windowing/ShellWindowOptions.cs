using System.Collections.Generic;
using System.Linq;

namespace LionFire.Shell
{

    public class ShellWindowOptions
    {
        private string[] Args => System.Environment.GetCommandLineArgs();

        public bool MinimizeAllOnFullScreen { get; set; } = true;
        public bool UndoMinimizeAllOnRestore { get; set; } = true;

        /// <summary>
        /// Set to true to disable default Windows TitleBar and use the custom one.
        /// </summary>
        public bool UseCustomTitleBar { get; set; } = true;

        public int DefaultWindowWidth { get; set; } = 850;
        public int DefaultWindowHeight { get; set; } = 600;

        public bool StartMaximizedToFullScreen { get; internal set; }

        public bool StartInFullScreen
        {
            get
            {
                // ENH: Deprecate this logic with Args in favor of a proper CommandLine library, with common functionality for LionFire Shell
                if (Args.Contains("--full-screen")) return true;
                if (Args.Contains("--windowed")) return false;

                return StartMaximizedToFullScreen;
            }
        }

        //public bool IsFullScreenDefault => !DevMode.IsDevMode; // TODO: Different default based on DevMode

        // FUTURE: Default sizes for different modes: PC/Tablet/etc.
        //public virtual Size DefaultWindowedSize => new Size(1368, 768);

    }
}
