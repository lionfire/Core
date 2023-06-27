using System;
using System.Collections.Generic;
using LionFire.Execution;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public class InitializingLifetimeWrapper<T> : IHostLifetime
        where T : IHostLifetime
    {
        //IServiceProvider serviceProvider;
        T WrappedLifetime;
        IEnumerable<IInitializable3> initializers;
        //public IHostLifetime WrappedLifetime { get; set; }


        public InitializingLifetimeWrapper(T wrappedLifetime, IEnumerable<IInitializable3> initializers) {
            //this.serviceProvider = serviceProvider;
            this.WrappedLifetime = wrappedLifetime;// ?? new TValue();
            this.initializers = initializers;
        }

        //public InitializingLifetimeWrapper(IHostLifetime wrapped = null) { WrappedLifetime = wrapped ?? new ConsoleLifetime(); }

        public async Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (initializers != null)
            {
                await initializers.RepeatAllUntilNull(i => i.Initialize, cancellationToken);
            }
            await WrappedLifetime.WaitForStartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await WrappedLifetime.StopAsync(cancellationToken);
        }
    }
}
