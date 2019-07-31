using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public class ExtensionlessFsOBus : OBusBase<ExtensionlessFsOBus>

    {
        public override IEnumerable<string> UriSchemes
        {
            get { yield return ExtensionlessFileReference.UriScheme; }
        }

        public override IEnumerable<Type> ReferenceTypes
        {
            get { yield return typeof(ExtensionlessFileReference); }
        }

        public override IEnumerable<Type> HandleTypes => Enumerable.Empty<Type>();

        public override IOBase TryGetOBase(IReference reference) => ExtensionlessFsOBase.Instance;
        public override IReference TryGetReference(string referenceString) => ExtensionlessFileReference.TryCreate(referenceString);
    }
}
