using LionFire.Vos;
using LionFire.Persistence.Persisters;
using LionFire.Vos.Collections;

namespace LionFire.Hosting;

public class CollectionTypeFromVobNode : ICollectionTypeProvider<VobReference>
{
    public Type GetCollectionType(VobReference reference) => reference.GetVob().AcquireOwn<CollectionType>()?.Type;
}

