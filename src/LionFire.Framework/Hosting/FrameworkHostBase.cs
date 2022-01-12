using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Threading;

namespace LionFire.Hosting
{
    
#pragma warning disable CA1063 // Implement IDisposable Correctly
    /// <summary>
    /// Convenience for testing 
    /// </summary>
    public class FrameworkHostBase : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        protected readonly string[] args;

        #region State

        readonly CancellationTokenSource hostFinished = new CancellationTokenSource();
        Task task;

        #endregion

        public FrameworkHostBase(Action<IHostBuilder> configure = null, string[] args = null)
        {
            if (configure != null) ConfigureHostBuilder = configure;
            this.args = args;

            Run();
        }

        public virtual IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder(args).LionFire(b => b.Framework());

        public Action<IHostBuilder> ConfigureHostBuilder { get; set; } //= h => { };

        protected void Run()
        {
            var hostBuilder = CreateHostBuilder() ;
            ConfigureHostBuilder?.Invoke(hostBuilder);
            task = hostBuilder
                .RunAsync(async () =>
                {
                    await hostFinished.Token.WaitHandle.WaitOneAsync();
                    //hostFinished.Token.WaitHandle.WaitOne();
                });
        }


#pragma warning disable CA1063 // Implement IDisposable Correctly
        public virtual void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            hostFinished.Cancel();
            hostFinished.Dispose();
        }
    }
}
