namespace LionFire.Persistence.Merging
{
    public enum MergePolicy
    {
        Unspecified = 0,
        AlwaysOverwriteFromSource = 1 << 0,
        NeverOverwriteFromSource = 1 << 1,
    }

}
