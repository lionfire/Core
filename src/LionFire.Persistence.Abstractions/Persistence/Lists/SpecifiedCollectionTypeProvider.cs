#nullable enable
using LionFire.Referencing;
using System;

namespace LionFire.Persistence.Persisters
{
    public class SpecifiedCollectionTypeProvider<TReference> : ICollectionTypeProvider<TReference>
        where TReference : IReference
    {
        public Type GetCollectionType(TReference reference)
        {
            // TODO: Deserialize ..Collection.*
            // TODO: Deserialize ..Collection Type=""
            // TODO: Deserialize ..Collection <Type>
            //return typeof(object);
            throw new NotImplementedException();
        }
    }
}
