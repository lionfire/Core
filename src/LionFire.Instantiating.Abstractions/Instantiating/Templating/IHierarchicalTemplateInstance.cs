using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IHasChildren
    {
        IEnumerable<object> Children { get; }
    }

    public interface IHierarchicalTemplateInstance : ITemplateInstance
    {
        void Add(object child);
    }
}
