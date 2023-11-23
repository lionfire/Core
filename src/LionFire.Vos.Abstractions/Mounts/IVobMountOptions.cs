#nullable enable
using LionFire.MultiTyping;
using System;

namespace LionFire.Vos.Mounts;

public interface IVobMountOptions : IEquatable<IVobMountOptions>, IMultiTypable
{
    string Name { get; }
    bool IsExclusive { get; set; }
    bool IsSealed { get; set; }
    bool IsWritable { get; set; }
    bool MustBeManuallyEnabled { get; set; }
    //bool MountAtStartup { get; set; }
    //bool MountOnDemand { get; set; }
    bool TryCreateIfMissing { get; set; }

    int? ReadPriority { get; set; }
    int? WritePriority { get; set; }

    bool IsExclusiveWithReadAndWrite { get; set; }
    string RootName { get; set; }

    bool? IsOwnedByOperatingSystemUser { get; } // REVIEW - does this belong here?
    bool? IsVariableDataLocation { get; } // REVIEW - does this belong here?
    //IFlex Flex { get; set; }

    IMount? UpstreamMount { get; set; }
}