using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Resilience;

namespace LionFire.Hosting;

public static class ResilienceHostingX
{
    public static IServiceCollection AddRetryResiliencePolicy(this IServiceCollection services)
       => services.AddResiliencePipeline(ResiliencePolicies.Retry, static builder =>
       {
           // See: https://www.pollydocs.org/strategies/retry.html
           builder.AddRetry(new RetryStrategyOptions
           {
               BackoffType = DelayBackoffType.Linear,
               //ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>()
               Delay = TimeSpan.FromSeconds(0.2),
               MaxDelay = TimeSpan.FromSeconds(5),
               MaxRetryAttempts = 5,
           });

           // See: https://www.pollydocs.org/strategies/timeout.html
           //builder.AddTimeout(TimeSpan.FromSeconds(1.5));
       });
}
