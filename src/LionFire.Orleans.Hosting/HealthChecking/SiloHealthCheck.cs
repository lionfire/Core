using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Orleans.Runtime;
using System.Collections.Generic;

namespace LionFire.Orleans_.AspNetCore_
{
    public class SiloHealthCheck : IHealthCheck
    {
        private readonly IEnumerable<IHealthCheckParticipant> participants;

        private static long lastCheckTime = DateTime.UtcNow.ToBinary();

        public SiloHealthCheck(IEnumerable<IHealthCheckParticipant> participants)
        {
            this.participants = participants;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var thisLastCheckTime = DateTime.FromBinary(Interlocked.Exchange(ref lastCheckTime, DateTime.UtcNow.ToBinary()));

            foreach (var participant in this.participants)
            {
                if (!participant.CheckHealth(thisLastCheckTime, out var reason))
                {
                    return Task.FromResult(HealthCheckResult.Degraded(reason));
                }
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
