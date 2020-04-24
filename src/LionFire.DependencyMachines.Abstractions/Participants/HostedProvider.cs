﻿using Microsoft.Extensions.Hosting;

namespace LionFire.DependencyMachines
{
    /// <summary>
    /// Wrapper for IHostedService to start/stop within a DependencyMachine.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HostedProvider<T> : Participant<HostedProvider<T>>
        where T : IHostedService
    {
        IHostedService HostedService { get; }

        public override string? DefaultKey => $"{{HostedProvider {typeof(T).FullName}}}";

        public HostedProvider(IHostedService hostedService)
        {
            HostedService = hostedService;
            StartTask = async (_, cancellationToken) =>
            {
                await HostedService.StartAsync(cancellationToken);
                return null;
            };
            StopTask = async (_, cancellationToken) =>
            {
                await HostedService.StopAsync(cancellationToken);
                return null;
            };
        }
    }
}