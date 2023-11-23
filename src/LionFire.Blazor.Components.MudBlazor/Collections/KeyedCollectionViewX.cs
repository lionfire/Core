using LionFire.Data.Mvvm;
using LionFire.FlexObjects;

namespace LionFire.Blazor.Components;

public static class KeyedCollectionViewX
{
    public static ShowDetailsVM<TKey, TValue, TValueVM> ShowDetailsVM<TKey, TValue, TValueVM>(this KeyedCollectionView<TKey, TValue, TValueVM> v)
        where TKey : notnull
    {
        return v.TryGetComponent<ShowDetailsVM<TKey, TValue, TValueVM>>();
    }
}
