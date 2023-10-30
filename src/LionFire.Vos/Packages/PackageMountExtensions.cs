using LionFire.Referencing;
using LionFire.Vos.Mounts;

namespace LionFire.Vos.Packages;

public static class PackageMountExtensions
{
    public static PackageInfo GetPackage(this Mount mount)
    {
        return mount.Target.GetChildSubpath(".{}/PackageInfo").GetReadHandle<PackageInfo>().Get().Result.Value;
    }
}
