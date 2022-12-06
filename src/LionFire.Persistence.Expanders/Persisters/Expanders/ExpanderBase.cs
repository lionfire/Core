using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persisters.Expanders;

public class ExpanderBase : IExpander
{
    public Type? SourceReadType()
    {
        throw new NotImplementedException();
    }

    public Type? SourceReadTypeForReference(IReference reference)
    {
        throw new NotImplementedException();
    }

    public IReadHandle? TryGetReadHandle(IReference sourceReference)
    {
        throw new NotImplementedException();
    }
}

