using System;

namespace LionFire.Vos.Collections
{
   
    public interface ICollectionTypeProvider
    {
        Type GetCollectionType(IVob vob);
    }
}
