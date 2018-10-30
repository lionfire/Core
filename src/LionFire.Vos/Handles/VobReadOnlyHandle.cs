using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Ontology;
using LionFire.Referencing;

namespace LionFire.Vos
{
    [ReadOnlyEditionFor(typeof(VobReadOnlyHandle<>))]
    public class VobReadOnlyHandle<T> : RBase<T>
    {
        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    }
}
