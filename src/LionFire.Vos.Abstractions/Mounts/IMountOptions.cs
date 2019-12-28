namespace LionFire.Vos.Mounts
{
    public interface IMountOptions
    {
        bool IsExclusive { get; set; }
        bool IsSealed { get; set; }
        bool IsWritable { get; set; }
        bool MountAtStartup { get; set; }
        bool MountOnDemand { get; set; }
        bool TryCreateIfMissing { get; set; }
    }
}