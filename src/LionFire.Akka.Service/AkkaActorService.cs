using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Akka.Service
{
    public class AkkaActorService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
