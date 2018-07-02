namespace LionFire.Collections
{

    public interface INotifyingReadOnlyList<ChildType> :
        IReadOnlyList<ChildType>,
        INotifyCollectionChanged<ChildType>
    {
    }

}
