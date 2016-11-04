using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Templating
{
    public interface ITemplateInstance
    {
        object Template { get;  set; }
    }
    public interface ITemplateInstance<T> : ITemplateInstance
    {
        new T Template { get;  set; }
    }
}
