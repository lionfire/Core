using LionFire.Orleans_.Collections;
using ObservableCollections;
using Orleans;
using static LionFire.Reflection.GetMethodEx;
using System.Collections.ObjectModel;

namespace LionFire.Orleans_.Mvvm;

/// <summary>
/// Writable Async Cache for an IListGrain
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListGrainCache<TCollection, TItem> : EnumerableGrainCache<TCollection, TItem>
    where TCollection : IListGrain<GrainListItem<TItem>>
    where TItem : class, IGrain
{
    #region Lifecycle

    public ListGrainCache(TCollection source, IGrainFactory grainFactory, IClusterClient clusterClient) : base(source, grainFactory, clusterClient)
    {
    }

    #endregion

    #region Collection

    public override bool IsReadOnly => false;

    #endregion

    #region Create

    public override bool CanCreate => true;

    public override async Task<GrainListItem<TItem>> Create(Type type, params object[]? constructorParameters)
    {
        if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
        if (!type.IsAssignableTo(typeof(TItem))) { throw new ArgumentException($"type must be assignable to {typeof(TItem).FullName}"); }
        if (constructorParameters != null && constructorParameters.Length != 0) { throw new ArgumentException($"{nameof(constructorParameters)} not supported"); }
        var result = await Source.Create(type);
        return result;
    }

    #endregion

    #region Add / Remove

    public override async Task<bool> Remove(GrainListItem<TItem> item)
    {
        if (collection == null) { throw new InvalidOperationException($"Cannot invoke while {nameof(Collection)} is null"); }
        
        var removedFromInternal = collection.Remove(item);
        
        if (Source == null)
        {
            return removedFromInternal;
        }
        else
        {
            bool removedFromSource = false;
            try
            {
                removedFromSource = await Source.Remove(item.Id);
            }
            catch
            {
                removedFromSource = false;
                try
                {
                    collection.Add(item);
                }
                catch { } // EMPTYCATCH
                throw;
            }
            return removedFromSource;
        }
    }


    #endregion
    //    #region View

    //    public ISynchronizedView<TModel, TViewModel> CreateSortedViewX()
    ////Func<TModel, string> identitySelector, Func<TModel, TViewModel> transform, IComparer<T> comparer)
    //    {
    //        //IObservableCollection<string> a = this;

    //        return Collection.CreateSortedView<TModel, string, TViewModel>(m => m.GetPrimaryKeyString(), m => ViewModelProvider.CreateViewModel<TViewModel>(m)!, Comparer<TModel>.Default);
    //    }
    //    public ISynchronizedView<TModel, TViewModel> CreateSortedViewY()
    //        //IObservableCollection<TModel> source, Func<TModel, string> identitySelector, Func<TModel, TViewModel> transform, IComparer<TViewModel> viewComparer)
    //    {
    //        return Collection.CreateSortedView<TModel, string, TViewModel>(m => m.GetPrimaryKeyString(), m => ViewModelProvider.CreateViewModel<TViewModel>(m), Comparer<TViewModel>.Default);
    //    }

    //    #endregion
}
