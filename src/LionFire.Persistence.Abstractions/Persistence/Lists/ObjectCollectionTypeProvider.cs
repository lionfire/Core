#nullable enable
using LionFire.Referencing;
using System;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Fallback provider: say everything is an object.
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    public class ObjectCollectionTypeProvider<TReference> : ICollectionTypeProvider<TReference>
        where TReference : IReference

    {
        public Type GetCollectionType(TReference reference) => typeof(object);
    }
}
