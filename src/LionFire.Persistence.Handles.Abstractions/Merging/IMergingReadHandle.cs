namespace LionFire.Persistence.Merging
{
    public interface IMergingReadHandle<out T> : IReadHandleBase<T>
    {
        MergePolicy MergePolicy { get; set; }
    }

}
