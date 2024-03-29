﻿using LionFire.Data.Collections;
using LionFire.Data.Async.Gets;

namespace LionFire.Orleans_.Collections;

/// <remarks>
/// Adds back get method editions of properties
/// </remarks>
public interface ICollectionG<TItem> : IAsyncCollectionBase_OLD<TItem>, IStatelessGetterG<IEnumerable<TItem>>
{
    Task<int> GetCount();
    Task<bool> GetIsReadOnly();
}

public static class ICollectionGX
{
    public static IAsyncCollection_OLD<TItem> ToAsyncCollection<TItem>(this ICollectionG<TItem> c) => new CollectionGAdapter<TItem>(c);
}

internal class CollectionGAdapter<TItem> : IAsyncCollection_OLD<TItem>
{
    private ICollectionG<TItem> c;

    public CollectionGAdapter(ICollectionG<TItem> c)
    {
        this.c = c;
    }

    public Task<bool> IsReadOnly => c.GetIsReadOnly();

    public Task<int> Count => c.GetCount();

    public Task Add(TItem item)
    {
        return c.Add(item);
    }

    public Task Clear()
    {
        return c.Clear();
    }

    public Task<bool> Contains(TItem item)
    {
        return c.Contains(item);
    }

    //public Task CopyTo(TItem[] array, int arrayIndex)
    //{
    //    return c.CopyTo(array, arrayIndex);
    //}

    public Task<IEnumerable<TItem>> GetEnumerableAsync()
    {
        return c.GetEnumerableAsync();
    }

    public Task<bool> Remove(TItem item)
    {
        return c.Remove(item);
    }
}
