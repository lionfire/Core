using System.Collections.Generic;

namespace LionFire.Collections
{
    //    public interface INotifyListChanged<ChildType> FUTURE - for efficient list updates in WPF?
    //{
    //    event NotifyListChangedHandler<ChildType> CollectionChanged;
    //}

    // TODO: Deprecate IReadOnlyCollection in favor of IReadOnlyList
    public interface INotifyingReadOnlyCollection<out T> : IReadOnlyCollection<T>, INotifyCollectionChanged<T>
    {
    }

}
