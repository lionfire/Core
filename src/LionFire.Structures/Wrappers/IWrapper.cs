namespace LionFire.Structures
{
    /// <summary>
    /// Typically combined with IReadWrapper and/or IWriteWrapper
    /// </summary>
    public interface IWrapper
    {
        bool HasValue { get; }
    }
}
