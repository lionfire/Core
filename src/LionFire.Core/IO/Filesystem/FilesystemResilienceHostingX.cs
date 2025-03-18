using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Polly;
using Polly.Retry;
using System;
using LionFire.IO;

namespace LionFire.Hosting;

public static class FilesystemResilienceHostingX
{
    public static IServiceCollection AddFilesystemResilience(this IServiceCollection s) => s
        .AddResilienceEnricher() // For Telemetry
        .AddDefaultFilesystemResiliencePolicies()
    ;


    // See: https://www.pollydocs.org/strategies/retry.html
    // See: https://www.pollydocs.org/strategies/timeout.html

    public static IServiceCollection AddDefaultFilesystemResiliencePolicies(this IServiceCollection services)
       => services
        .AddResiliencePipeline(FilesystemRetryPolicy.Default, static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                // "being used by another process"
                ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(200),
                MaxDelay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 10,
            });
            builder.AddTimeout(TimeSpan.FromSeconds(1.5));
        })

        //.AddResiliencePipeline(/* OLD */, static builder =>
        //{
        //    builder.AddRetry(new RetryStrategyOptions
        //    {
        //        BackoffType = DelayBackoffType.Linear,
        //        //ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>()
        //    });
        //    //builder.AddTimeout(TimeSpan.FromSeconds(1.5));
        //})

    #region OnFileChange


        .AddResiliencePipeline(FilesystemRetryPolicy.OnFileChange.Quick, static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                // "being used by another process"
                ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(20),
                MaxDelay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 10,
            });
            builder.AddTimeout(TimeSpan.FromSeconds(0.8));
        })

        .AddResiliencePipeline(FilesystemRetryPolicy.OnFileChange.Slow, static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                // "being used by another process"
                ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(100),
                MaxDelay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 15,
            });
            builder.AddTimeout(TimeSpan.FromSeconds(0.9));
        })

        .AddResiliencePipeline(FilesystemRetryPolicy.OnFileChange.Slow, static builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                // "being used by another process"
                ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(100),
                MaxDelay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 25,
            });
            builder.AddTimeout(TimeSpan.FromSeconds(0.8));
        })

    #endregion


        
        ;
}
