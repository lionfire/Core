namespace LionFire.Structures
{
    /// <summary>
    /// Writable version of IKeyed
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IKeyable<TKey> : IKeyed<TKey>
    {
        new TKey Key { get; set; }
    }

    public interface IKeyable : IKeyable<string> { }
}
