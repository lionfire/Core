using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface ITemplateInstance
    {
        ITemplate Template { get;  set; }
    }
    public interface ITemplateInstance<T> : ITemplateInstance
        where T : ITemplate
    {
        new T Template { get;  set; }
    }
}
