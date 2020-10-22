using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using LionFire.Ontology;
using System.Threading.Tasks;
using LionFire.Execution;
using System.Threading;

namespace LionFire.Wpf
{
    public class HostApplication : Application
    {
        // Non-generic type needed for WPF compiler so it can find Dependency Properties on base class
    }

    public abstract class HostApplication<THostProvider> : HostApplication
        where THostProvider : IHas<IHost>
    {
        public async Task<IHost> CreateHost(StartupEventArgs e, CancellationToken cancellationToken = default)
        {
            var builder = (THostProvider)Activator.CreateInstance(typeof(THostProvider), new object[] { e.Args });
            if (builder is IStartable startable) { await startable.StartAsync(cancellationToken).ConfigureAwait(false); }
            else if(builder is IHostedService hs) { await hs.StartAsync(cancellationToken).ConfigureAwait(false); }
            else if (builder is ITryStartable tryStartable) { await tryStartable.TryStartAsync(cancellationToken).ConfigureAwait(false); }
            return builder.Object;
        }

        public static int ShutdownTimeoutSeconds = 15;

        public HostApplication()
        {
            this.Startup += Application_Startup;
            this.Exit += Application_Exit;
        }

        internal IHost Host { get; private set; }
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Host = await CreateHost(e).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Shutdown(-1);
                return;
            }
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (Host)
            {
                await Host.StopAsync(TimeSpan.FromSeconds(ShutdownTimeoutSeconds));
            }
        }
    }
}
