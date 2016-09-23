using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Templating
{
    public interface IHierarchicalTemplate
    {
        List<ITemplate> Children { get; set; }
    }

    public interface IHierarchicalTemplate<T> : ITemplate<T>, IHierarchicalTemplate
        where T : IHierarchicalTemplateInstance, new()
    {
    }
    
}
