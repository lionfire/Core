using LionFire.Structures;
using Microsoft.Extensions.Hosting;

namespace LionFire.DependencyMachine
{
    public class DependencyProvider<T> : ResolvableDependency, IReadWrapper<T>
    {
        public override string Key => typeof(T).FullName;

        public T Value { get; protected set; }

    }

    public class HostedServiceDependencyProvider<T> : ResolvableDependency
        where T : IHostedService
    {
        IHostedService HostedService { get; }
        public override string Key => typeof(T).FullName;

        public T Value { get; protected set; }

        public HostedServiceDependencyProvider(IHostedService hostedService)
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
