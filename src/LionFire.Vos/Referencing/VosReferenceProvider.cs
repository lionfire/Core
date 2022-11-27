using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Vos;

public class VobReferenceProvider : ReferenceProviderBase<VobReference>
{
    public override string UriScheme => "vos";

    public override (VobReference reference, string error) TryGetReference(string path)
        => (new VobReference(path), null);
}
