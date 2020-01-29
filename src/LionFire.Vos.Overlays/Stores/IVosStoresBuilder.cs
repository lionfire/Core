using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;

namespace LionFire.Services
{
    public interface IVosStoresBuilder
    {
        IVosStoresBuilder AddStore(string storeName, IReference target, MountOptions dataMountOptions = null, MountOptions availablePackageMountOptions = null, string rootName = VosConstants.DefaultRootName);
    }
}
