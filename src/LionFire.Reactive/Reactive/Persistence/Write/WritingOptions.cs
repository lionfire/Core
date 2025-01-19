namespace LionFire.Reactive.Persistence;

public class WritingOptions
{
    public bool AutoSave { get; set; } = true;
    public TimeSpan DebounceDelay { get; set; } = TimeSpan.FromMilliseconds(500);

}
