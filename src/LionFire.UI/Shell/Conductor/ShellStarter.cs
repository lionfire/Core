using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Alerting;
using LionFire.Execution;

namespace LionFire.Shell.Wpf
{
    public class ShellStarter : IStartable
    {
        #region Dependencies

        IOptionsMonitor<ShellStartupOptions> ShellStartupOptions;
        public IShellConductor ShellConductor { get; }

        #endregion

        #region Construction

        public ShellStarter(IOptionsMonitor<ShellStartupOptions> shellStartupOptions, IShellConductor shellConductor)
        {
            ShellStartupOptions = shellStartupOptions;
            ShellConductor = shellConductor;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return ShowStartupInterfaces();
        }

        #endregion

        /// <summary>
        /// Invoked once at startup to bring up primary views
        /// </summary>
        public async Task ShowStartupInterfaces()
        {
            try
            {
                await Task.WhenAll(ShellStartupOptions.CurrentValue.StartupViews.Select(sv => ShellConductor.Show(sv))).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Alerter.Alert("Failed to show startup interface", ex);
            }

#if Windowing && WPF
            MainPresenter.Show(); // Show the window
            this.Shell.Application.MainWindow = MainPresenter.CurrentWindow;
#endif
        }
    }
}

#region OLD - startup task
//Func<Task> startup; // StartAsync waits for ctor tasks to finish
//startup = () => Task.Run(async () =>
//{
//    WindowSettings = await windowSettings.GetNonDefaultValue().ConfigureAwait(false);
//    // ENH Make this a Participant that contributes to CanStartShell?
//});

//await startup().ConfigureAwait(false);
//startup = null;
#endregion
