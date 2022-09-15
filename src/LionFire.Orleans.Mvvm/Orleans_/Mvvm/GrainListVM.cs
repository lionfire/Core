using LionFire.Mvvm;
using LionFire.Orleans_.Collections;
using Orleans;

namespace LionFire.Orleans_.Mvvm;

/// <summary>
/// Edits a Grain that contains a list of other grains that 
/// </summary>
/// <typeparam name="T"></typeparam>
public class GrainListVM<T> : ObservableListVM<T>
    where T : IGrain
{
    public IListGrain<T> Source { get; }
    public IGrainFactory GrainFactory { get; }

    public GrainListVM(IListGrain<T> source, IGrainFactory grainFactory)
    {
        Source = source;
        GrainFactory = grainFactory;
    }

    public override async Task<IEnumerable<T>> Retrieve(CancellationToken cancellationToken = default)
        => (await Source.Items()).Select(i => (T)GrainFactory.GetGrain(i.Type, i.Id));

    public override bool IsReadOnly => false;

    public override Task Add(T item)
    {
        return base.Add(item);
    }

    public override async Task<bool> Remove(T item)
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
                removedFromSource = await Source.Remove(item.GetPrimaryKeyString());
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
}
