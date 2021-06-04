namespace LionFire.UI
{
    public interface IReadOnlyDictionary3<TKey, out TValue>
    {

    }

    public interface IReadOnlyUICollection<TChild> : IReadOnlyDictionary3<string, TChild>
        where TChild : IUIKeyed
    {

    }

}
