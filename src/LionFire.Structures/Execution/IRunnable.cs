﻿using LionFire.Threading;

namespace LionFire.Execution;

public interface IRunnable : IStartable
{
    //CancellationToken Terminated { get; }
    Task RunTask { get; }
}

public static class IRunnableX
{
    public static async Task Run(this IRunnable @this, CancellationToken cancellationToken = default)
    {
        await @this.StartAsync(cancellationToken);
        await Task.Run(async () => await @this.RunTask, cancellationToken);
    }
}
