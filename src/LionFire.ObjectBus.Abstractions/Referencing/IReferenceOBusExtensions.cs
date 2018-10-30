using System;
using System.Collections.Generic;
using System.Text;
using LionFire.ObjectBus;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.ObjectBus
{
    public static class IReferenceOBusExtensions
    {
        public static bool IsResolvedReferenceType(IReference reference) => reference is IHas<IOBase> || reference is IHas<IOBus>;
    }
}
