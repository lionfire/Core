using Microsoft.Extensions.Hosting;

namespace LionFire.Shell.Wpf
{
    public class StopApplicationOnShellClosed
    {
        public StopApplicationOnShellClosed(IShellConductor shellConductor, IHostApplicationLifetime hostApplicationLifetime)
        {
            shellConductor.Closed += _ => hostApplicationLifetime.StopApplication();
            
        }
    }

}

