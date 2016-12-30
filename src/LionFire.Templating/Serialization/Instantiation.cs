using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Templating
{
    public class Instantiation
    {
        public ITemplate Template { get; set; }

        public string TemplateSubPath { get; set; }
        public Type TemplateType { get; set; }
        public Dictionary<string, object> State { get; set; }
    }


}
