using DynamicData;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Blazor.Components.Terminal;

public partial class TerminalView
{
    [Parameter]
    public LogVM ViewModel { get; set; } = new();

    [Parameter]
    public string? Style { get; set; }
}

public readonly struct LogEntry
{
    public readonly DateTimeOffset Timestamp;
    public readonly DateTimeOffset LocalTimestamp => Timestamp.ToLocalTime();
    public readonly LogLevel Level;
    public string LevelAbbreviation
            => Level switch
            {
                LogLevel.Trace => "TR",
                LogLevel.Debug => "DB",
                LogLevel.Information => "IN",
                LogLevel.Warning => "WN",
                LogLevel.Error => "ER",
                LogLevel.Critical => "CR",
                _ => "???",
            };

    public readonly string Category;
    public readonly string ShortCategory => Category?.Substring(Category.LastIndexOf('.') + 1) ?? string.Empty;

    public readonly string Text;
    public readonly int Index;

    public LogEntry(DateTimeOffset timestamp, LogLevel level, string text, int index, string category = "")
    {
        Timestamp = timestamp;
        Level = level;
        Category = category;
        Text = text;
        Index = index;
    }
}

public class LogVM : ILogger
{
    #region Parameters

    public int MaxBufferSize { get; set; } = 1000;
    public bool IsEnabled { get; set; } = true;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    public bool Reverse { get; set; } = true;

    public TimeSpan Debounce
    {
        get => debounce; set
        {
            if (debounce == value) return;

            debounceSubscription?.Dispose();
            debounceSubscription = null;

            debounce = value;
            if (value != TimeSpan.Zero)
            {
                debounceSubscription = debouncedChanges
                    .Throttle(value)
                    .Subscribe(x => _Append(x.Item1, x.Item2, x.Item3));
            }
        }
    }
    private TimeSpan debounce;
    IDisposable? debounceSubscription;

    #endregion

    public LogVM()
    {
        Debounce = TimeSpan.FromSeconds(2.0);
    }

    public IObservableCache<LogEntry, int> Lines => sourceCache;
    private SourceCache<LogEntry, int> sourceCache = new SourceCache<LogEntry, int>(x => x.Index);

    int index = 0;
    public void Append(string line, LogLevel logLevel = LogLevel.Information, string category = "")
    {
        if (Debounce == TimeSpan.Zero)
        {
            _Append(line, logLevel, category);
        }
        else
        {
            debouncedChanges.OnNext((line, logLevel, category));
        }
    }
    Subject<(string, LogLevel, string)> debouncedChanges = new();
    private void _Append(string line, LogLevel logLevel = LogLevel.Information, string category = "")
    {
        sourceCache.Edit(u =>
        {
            u.AddOrUpdate(new LogEntry(DateTimeOffset.UtcNow, logLevel, line, Reverse ? index-- : index++, category));
            var items = sourceCache.Items;

            while (sourceCache.Count > MaxBufferSize)
            {
                u.Remove((Reverse ? items.Last() : items.First()).Index);
            }
        });
    }

    public void Clear()
    {
        sourceCache.Clear();
    }


    #region ILogger

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled) return;
        if (logLevel < LogLevel) return;
        Append(formatter(state, exception), logLevel, this.GetType().FullName!);
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        if (!IsEnabled) return false;
        return logLevel >= LogLevel;
    }

    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    #endregion
}