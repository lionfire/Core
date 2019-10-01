using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating.Templating
{
    public class TemplateRestorer : IInstantiator
    {
        public ITemplate Template { get; set; }

        public object Instance { get; private set; }

        #region Construction

        public TemplateRestorer() { }
        public TemplateRestorer(ITemplate template, object instance) { this.Template = template; this.Instance = instance; }

        #endregion

        public object Affect(object obj, InstantiationContext context = null)
        {
            if (obj != null)
            {
                throw new InvalidOperationException("Object must not exist for a TemplateRestorer");
            }

            obj = this.Template.Create();

            return obj;
        }
    }
}
