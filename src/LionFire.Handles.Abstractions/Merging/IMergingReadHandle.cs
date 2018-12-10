namespace LionFire.Referencing
{
    public interface IMergingReadHandle<out T> : RH<T>
    {
        MergePolicy MergePolicy { get; set; }
    }

}
