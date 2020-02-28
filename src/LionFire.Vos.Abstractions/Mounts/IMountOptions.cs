namespace LionFire.Vos.Mounts
{
    public interface IMountOptions
    {
        bool IsExclusive { get; set; }
        bool IsSealed { get; set; }
        bool IsWritable { get; set; }
        bool IsManuallyEnabled { get; set; }
        //bool MountAtStartup { get; set; }
        //bool MountOnDemand { get; set; }
        bool TryCreateIfMissing { get; set; }

        int? ReadPriority { get; set; }
        int? WritePriority { get; set; }

        bool IsExclusiveWithReadAndWrite { get; set; }
        string RootName { get; set; }

        //IFlex Flex { get; set; }
    }
}