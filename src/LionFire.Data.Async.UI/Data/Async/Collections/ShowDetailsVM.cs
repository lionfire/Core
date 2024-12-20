﻿//using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace LionFire.Data.Mvvm;

public class ShowDetailsVM<TKey, TValue, TValueVM>
    where TKey : notnull
    where TValueVM : notnull
{
    public AsyncKeyedCollectionVM<TKey, TValue, TValueVM> Parent { get; }

    #region Parameters

    public bool ExpandMultipleDetail { get; set; }

    #endregion

    public ShowDetailsVM(AsyncKeyedCollectionVM<TKey, TValue, TValueVM> parent)
    {
        Parent = parent;
    }
    /// <summary>
    /// A collection of Keys to show detail for
    /// </summary>
    public HashSet<TKey> ShowDetailsFor { get; } = new();

    public bool ShouldShow(TKey key) => ShowDetailsFor.Contains(key);

    public void ToggleShowDetail(TKey key)
    {
        if (!ShowDetailsFor.Remove(key))
        {
            if (!ExpandMultipleDetail) { ShowDetailsFor.Clear(); }
            ShowDetailsFor.Add(key);
        }
    }

    //public void ToggleShowDetail(TValue item)
    //{
    //    ToggleShowDetail(Parent.GetKeyForItem(item));
    //}

}

public static class ShowDetailsVMX
{
    public static ShowDetailsVM<TKey, TValue, TValueVM> ShowDetailsVM<TKey, TValue, TValueVM>(this IComponentized componentized)
        where TKey : notnull
    {
        return componentized.TryGetComponent<ShowDetailsVM<TKey, TValue, TValueVM>>();
    }
}
