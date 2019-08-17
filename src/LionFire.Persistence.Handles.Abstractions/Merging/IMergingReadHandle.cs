namespace LionFire.Persistence.Merging
{
    public interface IMergingReadHandle<out T> : RH<T>
    {
        MergePolicy MergePolicy { get; set; }
    }

}
