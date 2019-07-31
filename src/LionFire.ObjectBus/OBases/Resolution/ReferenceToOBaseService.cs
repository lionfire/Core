using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.DependencyInjection;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.ObjectBus
{
    // REVIEW
    // - consider wiring up to OBaseSchemeRegistrar to go from O(n) iterative lookup to a hash lookup
    //   - This would also provide validation to detect config errors and avoid undefined behavior.
    public class ReferenceToOBaseService : IReferenceToOBaseService
    {
        //public IEnumerable<IOBus> Providers { get; }
        public IEnumerable<IOBus> Providers
        {
            get
            {
                if (providers == null)
                {
                    providers = DependencyContext.Current.GetService<IEnumerable<IOBus>>(); // TODO FIXME - deterministic init flow
                }
                return providers;
            }
        }
        private IEnumerable<IOBus> providers;

        //public ReferenceToOBaseService(IEnumerable<IOBus> providers)
        //{
        //    Providers = providers;
        //}

        public IOBus GetFirstCompatible(IReference input) => DependencyContext.Current.GetService<IEnumerable<IOBus>>().Where(s => s.IsCompatibleWith(input)).FirstOrDefault();

        public (IOBus OBus, IOBase OBase) Resolve(IReference reference)
        {
            bool isDynamicReference = true;
            IOBase obase;

            if (reference is IHas<IOBase> hasOBase)
            {
                isDynamicReference = false;
                obase = hasOBase.Object;
                if (obase != null)
                {
                    return (obase?.OBus, obase);
                }
            }

            IOBus obus = null;

            if (reference is IHas<IOBus> hasOBus)
            {
                isDynamicReference = false;
                obus = hasOBus.Object;
            }

            if (obus != null)
            {
                obus = GetFirstCompatible(reference);
            }

            obase = obus?.TryGetOBase(reference);

            //if()

            if (isDynamicReference)
            {
                throw new NotImplementedException("TODO: Resolve dynamic reference type to specific reference type");
            }

            return (obus, obase);
        }

        public IOBus ResolveOBus(IReference reference)
        {
            bool isDynamicReference = true;

            IOBus obus = null;

            if (reference is IHas<IOBus> hasObaseProvider)
            {
                isDynamicReference = false;
                obus = hasObaseProvider.Object;
            }

            if (obus != null)
            {
                return obus;
            }

            IOBase obase;

            if (reference is IHas<IOBase> hasObase)
            {
                isDynamicReference = false;
                obase = hasObase.Object;
                return obase?.OBus;  // Returns if null
            }

            if (isDynamicReference)
            {
                throw new NotImplementedException("TODO: Resolve dynamic reference type to specific reference type");
            }

            return null;
        }
    }
}
