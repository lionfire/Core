namespace LionFire.Structures;

/// <summary>
/// Typically combined with IReadWrapper to indicate whether a value is available
/// </summary>
public interface IDefaultable
{
    bool HasValue { get; }
}
