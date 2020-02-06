namespace LionFire.Referencing
{
    /// <summary>
    /// Corresponds to the hostname and port portions of a URI
    /// </summary>
    public interface IHostReference
    {
        string Host { get; }
        string Port { get; }
    }
}
