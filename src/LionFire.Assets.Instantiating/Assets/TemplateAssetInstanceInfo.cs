using LionFire.Instantiating;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets.Instantiating
{

    public class TemplateAssetInstanceInfo<TTemplate, TParameters> : InstantiationAwareBase<TTemplate, TParameters>
        where TTemplate : class, ITemplateAsset
        where TParameters : class, ITemplateParameters
    {
    }

    public class TemplateAssetInstanceInfo<TTemplate> : ITemplateInstance<TTemplate, EmptyTemplateParameters>
        where TTemplate : class, ITemplateAsset

    {
        #region Parameters

        ITemplateParameters ITemplateInstance<TTemplate, EmptyTemplateParameters>.Parameters { get => null; set => throw new NotSupportedException(); }
        public EmptyTemplateParameters Parameters
        {
            get => Singleton<EmptyTemplateParameters>.Instance;
            set => throw new NotSupportedException();
        }

        #endregion

        #region Template

        //ITemplate ITemplateInstance.Template { get => Template; set => Template = (TTemplate)value; }
        //ITemplateAsset ITemplateAssetInstance.TemplateAsset { get { return Template; } set { Template = (TTemplate)value; } }

        public TTemplate Template
        {
            get => template;
            set
            {
                //if (template == value) return;
                if (template != null) throw new NotSupportedException("Can only be set once");
                template = value;
            }
        }
        private TTemplate template;

        #endregion
    }

}
