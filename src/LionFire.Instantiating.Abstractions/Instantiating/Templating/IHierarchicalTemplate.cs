using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IHierarchicalTemplate : ITemplate
    {
        IEnumerable<IInstantiation> Children { get; 
            //set;  // TOPORT if needed 
        }
    }

    public interface IHierarchicalTemplate<T> : ITemplate<T>, IHierarchicalTemplate
        where T : IHierarchicalTemplateInstance, new()
    {
    }
}
