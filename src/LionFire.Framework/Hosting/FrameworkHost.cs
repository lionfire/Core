using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Threading;

namespace LionFire.Hosting
{
#if TOVOSAPP
    public class VosHost : FrameworkHost
    {
        public override IHostBuilder CreateHostBuilder() => FrameworkHostBuilder.CreateVos(args);

        public VosHost(Action<IHostBuilder> configure = null) : base(configure) { }
    }
#endif

    public class FrameworkHost : IDisposable
    {

        protected readonly string[] args;

#region State

        readonly CancellationTokenSource hostFinished = new CancellationTokenSource();
        Task task;

#endregion

        public FrameworkHost(string[] args = null)
        {
            this.args = args;
        }

        public virtual IHostBuilder CreateHostBuilder() => FrameworkHostBuilder.Create(args);

        public Action<IHostBuilder> ConfigureHostBuilder { get; set; } = h => { };

        protected void Run()
        {
            var hostBuilder = CreateHostBuilder();
            ConfigureHostBuilder(hostBuilder);
            task = hostBuilder
                .Run(async () =>
                {
                    await hostFinished.Token.WaitHandle.WaitOneAsync();
                    //hostFinished.Token.WaitHandle.WaitOne();
                });
        }

        public FrameworkHost(Action<IHostBuilder> configure = null)
        {
            if (configure != null) ConfigureHostBuilder = configure;

            Run();
        }

        public virtual void Dispose()
        {
            hostFinished.Cancel();
            hostFinished.Dispose();
        }
    }
}
