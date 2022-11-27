using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Persisters.Expansion;

// Convention:    
// - ExpansionReference.TryParse
// - TODO: UriScheme
// - TODO .NET 7: go from convention to static interface  

public class ExpansionReferenceProvider : ReferenceProviderBase<ExpansionReference>
{
    public override string UriScheme => "expand";

}
