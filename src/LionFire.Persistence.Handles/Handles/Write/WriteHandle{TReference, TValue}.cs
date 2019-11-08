using System;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    public class WriteHandle<TReference, TValue> : WriteHandleBase<TReference, TValue>, IWriteHandle<TValue>
        where TReference : class, IReference
        where TValue : class
    {
        public override Task<IResolveResult<TValue>> ResolveImpl() => throw new NotImplementedException();
    }
}
