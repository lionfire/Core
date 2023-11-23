// REVIEW: Use ResolvesTracker instead? Avoid Mediatr
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Disposable;

public enum DisposableState
{
    Unspecified = 0,
    Created = 1 << 0,
    Disposed = 1 << 1,
}

public class DisposableNotification : INotification
{
    public (string type, string id) Key { get; set; }

    public DisposableState State { get; set; }
}

public class DisposableInfo : INotification
{
    public (string type, string id) Key { get; set; }

    public string Type => Key.type;
    public string Id => Key.id;
    public Dictionary<string, object> Properties { get; set; }

    public bool CanDispose { get; set; }
    public bool Cache { get; set; }

    public DisposableState State { get; set; }
}

public class DisposableTracker
{
    public ConcurrentDictionary<(string type, string id), DisposableInfo> Disposables { get; } = new();

}

public class DisposableTrackerHandler : INotificationHandler<DisposableNotification>, INotificationHandler<DisposableInfo>
{
    public DisposableTrackerHandler(DisposableTracker disposableTracker)
    {
        DisposableTracker = disposableTracker;
    }

    public DisposableTracker DisposableTracker { get; }

    public Task Handle(DisposableNotification notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Handle(DisposableInfo notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public static class DisposableTrackerX
{
    public static IServiceCollection AddDisposableTracker(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(DisposableTrackerHandler).Assembly);
        });
        services.AddSingleton<DisposableTracker>();
        return services;
    }
}
