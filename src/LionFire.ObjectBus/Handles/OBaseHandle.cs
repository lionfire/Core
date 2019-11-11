using System;
using System.Threading.Tasks;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.ObjectBus.Handles
{
    public class OBaseHandle<T> : ReadWriteHandleBase<T>, IHas<IOBase>
    {
        public IOBase OBase { get;  }
        IOBase IHas<IOBase>.Object => OBase;


        public OBaseHandle(IReference reference, IOBase obase, T handleObject = default) : base(reference, handleObject)
        {
            this.OBase = obase ?? throw new ArgumentNullException(nameof(obase));
        }

        // Some code duplication with OBaseReadHandle
        public override async Task<IResolveResult<T>> ResolveImpl() 
            => await OBase.Get<T>(this.Reference).ConfigureAwait(false);

        //protected async Task<IPersistenceResult> DeleteObject()
        //    => await OBase.TryDelete<T>(this.Reference).ConfigureAwait(false);

        protected override async Task<IPersistenceResult> DeleteObject()
            => await OBase.TryDelete<T>(this.Reference).ConfigureAwait(false);

        protected override async Task<IPersistenceResult> WriteObject()
        {
            // TODO: propagate persistenceContext?  Or remove it and rely on ambient AsyncLocal?
            var result = await OBase.Set<T>(this.Reference, ProtectedValue).ConfigureAwait(false);
            if (!result.IsSuccess()) throw new PersistenceException(result, "Failed to persist.  See Result for more information.");
            return result;

        }
    }
}
