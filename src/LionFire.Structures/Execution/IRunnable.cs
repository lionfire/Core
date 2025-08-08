using LionFire.Threading;

namespace LionFire.Execution;

public interface IRunnable : IStartable
{
    //CancellationToken Terminated { get; }
    Task RunTask { get; }
}
public interface ICancelableRunnable : IRunnable
{
    void AddCancellationToken(CancellationToken cancellationToken);
}


public static class IRunnableX
{
    public static async Task Run(this IRunnable @this, CancellationToken runCancellationToken = default, CancellationToken startCancellationToken = default)
    {
        await @this.StartAsync(startCancellationToken).ConfigureAwait(false);

        if(@this is ICancelableRunnable cancelableRunnable)
        {
            cancelableRunnable.AddCancellationToken(runCancellationToken);
        }

        await @this.RunTask.ConfigureAwait(false);
    }
}
