#nullable enable

namespace LionFire.Hosting;

public enum VobInitializerFlags
{
    None = 0,
    Contributes = 1 << 0,
    AfterParent = 1 << 1,
    DependsOnParent = 1 << 2,
    Default = Contributes | AfterParent,
}


