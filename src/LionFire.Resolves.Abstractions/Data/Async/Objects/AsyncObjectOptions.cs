
namespace LionFire.Data.Async;

public class AsyncObjectOptions
{
    /// <summary>
    /// Default options to use for child properties
    /// </summary>
    public AsyncValueOptions PropertyOptions { get; } = new();

    public bool AutoGet { get; set; }
}