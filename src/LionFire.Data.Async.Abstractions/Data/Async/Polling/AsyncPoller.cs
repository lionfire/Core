using LionFire.Threading;

namespace LionFire.Data.Mvvm;

public class AsyncPoller<T> : IDisposable
{
    #region Relationships

    public Func<Task> PollFunc { get; }

    #endregion

    #region Parameters

    public TimeSpan? PollDelay { get; set; }

    #endregion

    #region Lifecycle

    public AsyncPoller(Func<Task> pollFunc, TimeSpan pollDelay)
    {
        PollFunc = pollFunc;
        PollDelay = pollDelay;
        StartPolling();
    }

    public void Dispose()
    {
        StopPolling();
    }

    #endregion

    #region State

    PeriodicTimer? PollingPeriodicTimer;
    CancellationTokenSource? PollingCancellationTokenSource { get; set; }

    #endregion

    #region Methods

    public Task Poll() => PollFunc();


    private void StartPolling()
    {
        if (!PollDelay.HasValue) return;

        PollingCancellationTokenSource = new CancellationTokenSource();
        PollingPeriodicTimer = new PeriodicTimer(PollDelay.Value);
        Task.Run(async () =>
        {
            var cts = PollingCancellationTokenSource;
            while (!cts.IsCancellationRequested)
            {
                await PollingPeriodicTimer.WaitForNextTickAsync(cts.Token);
                await Poll();

            }
        }).FireAndForget();
    }

    private void StopPolling()
    {
        PollingCancellationTokenSource?.Cancel();
        PollingCancellationTokenSource = null;
        PollingPeriodicTimer = null;
    }



    #endregion

}
