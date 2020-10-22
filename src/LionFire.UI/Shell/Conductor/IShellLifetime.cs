using Microsoft.Extensions.Hosting;

namespace LionFire.Shell
{
    //public interface IShellLifetime : IHostedService { }

#if UNUSED
    public class ShellLifetime : IShellLifetime
    {
    #region Dependencies

        public IShellConductor ShellConductor { get; }

    #endregion

    #region Construction

        public ShellLifetime(IShellConductor shellConductor)
        {
            ShellConductor = shellConductor;
        }

    #endregion

    #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (ShellConductor is IHostedService hs)
            {
                await hs.StartAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                ShellConductor.ShowStartupInterfaces();
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (ShellConductor is IHostedService hs)
            {
                await hs.StopAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                ShellConductor.Close();
            }
        }
        
    #endregion
    }
#endif


}
