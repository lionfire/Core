using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{
    [ReadOnlyEditionIs(typeof(VobReadOnlyHandle<>))]
    public class VobHandle<T> : WBase<T>
    {
        public override Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
        public override Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
    }
}
