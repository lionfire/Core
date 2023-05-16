using LionFire.Structures;

namespace LionFire.Collections.Async;


public interface IAsyncListBase<TItem> : IAsyncCollectionBase<TItem>
{
    Task<TItem> ElementAt(int index);
    Task ElementAt(int index, TItem value);

    Task<int> IndexOf(TItem item);

    Task Insert(int index, TItem item);
    
    Task RemoveAt(int index);
}


public interface IAsyncList<TItem> : IAsyncCollection<TItem>, IAsyncListBase<TItem>
{
}

