#if CySharp_ObservableCollections
using ObservableCollections;

namespace LionFire.Mvvm;

public static class ListX
{
    public static void SyncTo<T>(this IEnumerable<T> source, IObservableCollection<T> destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ICollection<T>? collection = destination as ICollection<T>;
        if (collection != null)
        {
            foreach (var removal in collection.Where(i => !source.Contains(i)).ToArray())
            {
                collection.Remove(removal);
            }
        }
        else
        {
            throw new NotSupportedException($"{destination.GetType().FullName}");
        }

        if (destination is ObservableList<T> list)
        {
            list.AddRange(source.Where(i => !collection.Contains(i)).ToArray());
        }
        else
        {
            foreach (var addition in source.Where(i => !collection.Contains(i)))
            {
                collection.Add(addition);
            }
        }
    }

    public static void SyncTo<T>(this IEnumerable<T> source, ObservableHashSet<T> destination)
        where T : notnull
    {
        foreach (var removal in destination.Where(i => !source.Contains(i)).ToArray())
        {
            destination.Remove(removal);
        }
        destination.AddRange(source.Where(i => !destination.Contains(i)));
    }
}
#endif