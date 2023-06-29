using LionFire.Collections;
using LionFire.ExtensionMethods.Dependencies;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Gets;

public class ResolvesTracker : IHostedService
{
    public ConcurrentDictionary<Type, ConcurrentWeakDictionaryCache<string, object>> Types => types;
    ConcurrentDictionary<Type, ConcurrentWeakDictionaryCache<string, object>> types = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        types = new();
        LazilyGetsEvents.ValueChanged += LazilyResolvesEvents_ValueChanged;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        LazilyGetsEvents.ValueChanged -= LazilyResolvesEvents_ValueChanged;
        types = null;
        return Task.CompletedTask;
    }

    private void LazilyResolvesEvents_ValueChanged((Type valueType, ILazilyGets resolves, object from, object to) args)
    {
        IKeyed<string> stringKeyed = args.resolves as IKeyed<string>;
        if (stringKeyed == null) return;

        var x = types.GetOrAdd(args.valueType, _ => new ConcurrentWeakDictionaryCache<string, object>());

        if (args.to != null)
        {
            x.GetOrAdd(stringKeyed.Key, () => args.resolves);
        }
        else
        {
            x.Remove(stringKeyed.Key);
        }
    }
}

public static class ResolvesTrackerX
{
    public static IServiceCollection AddResolvesTracker(this IServiceCollection services)
    {
        services.AddSingleton<ResolvesTracker>();
        services.AddHostedService<ResolvesTracker>(sp => sp.GetRequiredService<ResolvesTracker>());
        return services;
    }
}
