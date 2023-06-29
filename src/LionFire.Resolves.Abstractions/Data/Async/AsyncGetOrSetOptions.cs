
namespace LionFire.Data;

public class AsyncGetOrSetOptions
{
    /// <summary>
    /// Try to Dispose the cached Value when the object is disposed
    /// </summary>
    public bool DisposeValue { get; set; } = true;

    public bool DisposeKey { get; set; } = false;
}
