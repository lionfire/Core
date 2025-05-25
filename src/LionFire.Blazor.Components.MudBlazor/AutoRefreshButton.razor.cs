#nullable enable

using Microsoft.AspNetCore.Components;
using System.Text;

namespace LionFire.Blazor.Components.MudBlazor_;

public partial class AutoRefreshButton
{
    public int Count = 0;
    #region Parameters

    // FIXME: should be auto property
    [Parameter]
    public bool Auto
    {
        get => auto; 
        set
        {
            if (auto == value) return;

            auto = value;

            if (auto)
            {
                if((DateTimeOffset.UtcNow - LastRefresh).TotalMilliseconds > Interval / 2.0)
                {
                    Logger.LogInformation("Enabled timer but it hasn't been refreshed recently. Refreshing now.");
                    _ = Refresh();
                }
            }
        }
    }
    private bool auto = false;

    [Parameter]
    public double Interval { get; set; } = 5000;

    [Parameter]
    public Func<Task>? OnRefresh { get; set; } = null;

    #endregion

    public string Tooltip
    {
        get
        {
            var sb = new StringBuilder();
            if(LastRefreshElapsed != null)
            {
                sb.Append($"Last refresh: {LastRefreshElapsed.ElapsedMilliseconds}ms");
            }
            return sb.ToString();
        }
    }
    Stopwatch? LastRefreshElapsed;

    #region Initialization

    protected override Task OnInitializedAsync()
    {
        UpdateTimer();
        if (Auto && OnRefresh != null) { _ = Refresh(); }
        return base.OnInitializedAsync();
    }

    #endregion

    #region State

    protected System.Timers.Timer? Timer { get; set; }
    protected bool Refreshing => refreshing == 1;
    volatile int refreshing = 0;

    DateTimeOffset LastRefresh = default;


    #region Derived

    public bool CanRefresh => OnRefresh != null;
    public bool Errored => Error != null;

    #endregion

    public Exception? Error
    {
        get => error;
        set
        {
            if (error != value)
            {
                error = value;
                InvokeAsync(StateHasChanged);
            }
        }
    }
    private Exception? error;
    

    #endregion

    private void UpdateTimer()
    {
        if (Auto && !Errored)
        {
            Logger.LogTrace($"Enabling timer for {Interval}ms");
            if (Timer == null)
            {
                Timer = new System.Timers.Timer();
                Timer.Elapsed += OnTimer;
                Timer.AutoReset = false;
            }
            else
            {
                Timer.Stop();
            }
            Error = null;
            Timer.Interval = Interval;
            Timer.Start();
        }
        else
        {
            Logger.LogInformation($"Disabling timer{(Errored ? " (ERRORED)" : "")}");
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Elapsed -= OnTimer;
                Timer.Dispose();
                Timer = null;
            }
        }
    }

    public async Task Refresh()
    {
        Count++;
        if (Interlocked.CompareExchange(ref refreshing, 1, 0) != 0)
        {
            Logger.LogWarning("OnTimer: already refreshing. Skipping.");
            return;
        }

        try
        {
            if (OnRefresh != null)
            {
                var shc = InvokeAsync(StateHasChanged); // Updates Refreshing
                Task refreshTask;
                try
                {
                    LastRefresh = DateTimeOffset.UtcNow;
                    var sw = Stopwatch.StartNew();
                    refreshTask = InvokeAsync(() => OnRefresh());
                    await refreshTask.ConfigureAwait(false);
                    sw.Stop();
                    Logger.LogInformation($"Refresh took {sw.ElapsedMilliseconds}ms");
                    LastRefreshElapsed = sw;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "[Parameter] OnRefresh() method threw Exception");
                    Error = ex;
                }
                UpdateTimer();
                await shc.ConfigureAwait(false);
            }
        }
        finally
        {
            refreshing = 0;
            await InvokeAsync(StateHasChanged);
        }
    }


    private async void OnTimer(object? sender, System.Timers.ElapsedEventArgs args)
    {
        try
        {
            Logger.LogTrace($"OnTimer");
            await Refresh();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnTimer failed");
            Error = ex;
            UpdateTimer();
        }
    }

    private void OnToggleRefresh()
    {
        Auto ^= true;
        if (!CanRefresh)
        {
            Error = new Exception("No handler registered with OnRefresh parameter");
        }
        else
        {
            if (Errored)
            {
                Logger.LogInformation("Clearing error.");
                Error = null;
            }
        }
        UpdateTimer();
    }

    private void OnContextMenu()
    {
        Logger.LogInformation("OnContextMenu");
    }

    void IDisposable.Dispose() { Auto = false; UpdateTimer(); }

}