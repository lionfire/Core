using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    // Saves a bit of typing if you can use a base class
    // FUTURE: VS Snippet
    public class TemplateInstanceBase<TemplateType> : ITemplateInstance<TemplateType>
        where TemplateType : ITemplate
    {
        #region Identity

        ITemplate ITemplateInstance.Template { get { return Template; } set { Template = (TemplateType)value; } }
        //[SetOnce]
        public TemplateType Template { get; set; }

        #endregion

    }
}
