using Microsoft.Extensions.Hosting;

namespace LionFire.DependencyMachines
{
    public class HostedProvider<T> : Participant
        where T : IHostedService
    {
        IHostedService HostedService { get; }
        public override string Key => typeof(T).FullName;

        public T Value { get; protected set; }

        public HostedProvider(IHostedService hostedService)
        {
            HostedService = hostedService;
            StartAction = async (_, cancellationToken) =>
            {
                await HostedService.StartAsync(cancellationToken);
                return null;
            };
            StopAction = async (_, cancellationToken) =>
            {
                await HostedService.StopAsync(cancellationToken);
                return null;
            };
        }
    }
}
