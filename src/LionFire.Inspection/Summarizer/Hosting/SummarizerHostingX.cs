using LionFire.Inspection;
using LionFire.Summarizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class SummarizerHostingX
{
    public static IServiceCollection AddSummarizer(this IServiceCollection services, bool? useDefaults = true)
    {
        services
            .AddSingleton<ISummarizerService, SummarizerService>()
            ;

        if (useDefaults == true)
        {
            services.AddDefaultSummarizers();
        }
        return services;
    }

    public static IServiceCollection AddDefaultSummarizers(this IServiceCollection services)
    {
        return services
            .TryAddEnumerableSingleton<ISummarizer, DefaultSummarizer>()
            ;
    }
}
