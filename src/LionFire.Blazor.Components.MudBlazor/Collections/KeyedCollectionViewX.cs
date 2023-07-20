using LionFire.Data.Mvvm;

namespace LionFire.Blazor.Components;

public static class KeyedCollectionViewX
{
    public static ShowDetailsVM<TKey, TValue, TValueVM> ShowDetailsVM<TKey, TValue, TValueVM>(this KeyedCollectionView<string, TValue, TValueVM> v)
    {
        throw new NotImplementedException();
    }
}
