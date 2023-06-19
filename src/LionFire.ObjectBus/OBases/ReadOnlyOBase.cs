#define TRAGE_GET
using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public abstract class ReadOnlyOBase<ReferenceType> : OBase<ReferenceType>, IOBase
        where ReferenceType : class, IReference
    {
        private Exception ReadOnlyException => new NotSupportedException("Read only");


        public override Task<ITransferResult> Set<T>(ReferenceType reference, T obj, bool allowOverwrite = true) => throw ReadOnlyException;

        public override Task<ITransferResult> CanDelete<T>(ReferenceType reference) => Task.FromResult<ITransferResult>(PersistenceResult.PreviewFail);

    }
}
