using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;

namespace LionFire.Resilience
{
    public static class FilesystemResilience
    {
        public const string Retry = "Filesystem-Retry";

        // Example:
        //private void ExampleUsage()
        //{
        //    ResiliencePipelineProvider<string> pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();
        //    ResiliencePipeline pipeline = pipelineProvider.GetPipeline(FilesystemResilience.Retry);
        //}
    }
}

namespace LionFire.Hosting
{
    using LionFire.Resilience;

    public static class FilesystemResilienceHostingX
    {

        public static IServiceCollection AddFilesystemResiliencePolicy(this IServiceCollection services)
           => services.AddResiliencePipeline(FilesystemResilience.Retry, static builder =>
            {
                // See: https://www.pollydocs.org/strategies/retry.html
                builder.AddRetry(new RetryStrategyOptions
                {
                    BackoffType = DelayBackoffType.Linear,
                    //ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>()
                });

                // See: https://www.pollydocs.org/strategies/timeout.html
                //builder.AddTimeout(TimeSpan.FromSeconds(1.5));
            });
    }
}
