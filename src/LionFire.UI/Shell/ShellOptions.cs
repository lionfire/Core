using System.Diagnostics;
using System.Linq;

namespace LionFire.Shell
{


    public class ShellOptions : ShellWindowOptions // REFACTOR: Separate ShellWindowOptions, ShellConductorOptions
    {
        /// <summary>
        /// If true, Shell will invoke its own StartAsync method after IHostApplicationLifetime.ApplicationStarted fires.
        /// If false, start another way such as IServicesCollection.AddSingletonHostedServiceDependency<WpfShell>()
        /// </summary>
        public bool AutoStart { get; set; } = false;

        public SourceLevels DataBindingSourceLevel { get; set; } = System.Diagnostics.SourceLevels.Verbose; // MOVE?  WPF?
                
        public bool ShellIsClosedWhenKeepAlivePresentersClosed { get; set; } = true; 

    }
}
