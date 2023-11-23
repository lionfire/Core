using LionFire.Structures;

namespace LionFire.Data.Collections;


public interface IAsyncListBase<TItem> : IAsyncCollectionBase_OLD<TItem>
{
    Task<TItem> ElementAt(int index);
    Task ElementAt(int index, TItem value);

    Task<int> IndexOf(TItem item);

    Task Insert(int index, TItem item);
    
    Task RemoveAt(int index);
}


public interface IAsyncList<TItem> : IAsyncCollection_OLD<TItem>, IAsyncListBase<TItem>
{
}

