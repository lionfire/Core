using System;
using System.Threading.Tasks;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Persistence;

namespace LionFire.Referencing
{
    public class OBaseHandle<T> : WBase<T>, IHas<IOBase>
    {
        public IOBase OBase { get;  }
        IOBase IHas<IOBase>.Object => OBase;


        public OBaseHandle(IReference reference, IOBase obase) : base(reference)
        {
            this.OBase = obase ?? throw new ArgumentNullException(nameof(obase));
        }

        // Some code duplication with OBaseReadHandle
        public override async Task<bool> TryRetrieveObject()
        {
            var result = await OBase.TryGet(this.Reference).ConfigureAwait(false);
            return result?.IsRetrieved == true;
        }

        protected override async Task<IPersistenceResult> DeleteObject<T>(object persistenceContext = null) 
            => await OBase.TryDelete<T>(this.Reference).ConfigureAwait(false);

        protected override async Task WriteObject(object persistenceContext = null)
        {
            // TODO: propagate persistenceContext?  Or remove it and rely on ambient AsyncLocal?
            await OBase.Set(this.Reference, Object, typeof(T)).ConfigureAwait(false);
        }
    }
}
