namespace LionFire.Referencing
{
    public interface IMergingReadHandle<out T> : R<T>
    {
        MergePolicy MergePolicy { get; set; }
    }

}
