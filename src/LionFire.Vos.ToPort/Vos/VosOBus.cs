using System;
using System.Collections.Generic;
using LionFire.Vos;
using LionFire.Referencing;
using LionFire.ObjectBus;

namespace LionFire.Vos
{
    public class VosOBus : OBusBase<VosOBus>
    {
        public override IOBase DefaultOBase => LionFire.Vos.VBase.Default;

        public IEnumerable<Type> SupportedReferenceTypes
        {
            get
            {
                yield return typeof(VosReference);
            }
        }
        public bool IsValid(IReference reference) => throw new NotImplementedException();
    }
}
