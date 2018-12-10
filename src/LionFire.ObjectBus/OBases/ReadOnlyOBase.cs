#define TRAGE_GET
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public abstract class ReadOnlyOBase<ReferenceType> : OBase<ReferenceType>, IOBase
        where ReferenceType : class, IReference
        //where HandleInterfaceType : IHandle
    {
        private Exception ReadOnlyException => new NotSupportedException("Read only");

        public override Task Set(ReferenceType reference, object obj, Type type = null, bool allowOverwrite = true/*, bool preview = false*/) => throw ReadOnlyException;
        public override Task<bool?> CanDelete(ReferenceType reference) => Task.FromResult((bool?)false);
        public override Task<bool?> TryDelete(ReferenceType reference/*, bool preview = false*/) => Task.FromResult<bool?>(false);
    }
}
