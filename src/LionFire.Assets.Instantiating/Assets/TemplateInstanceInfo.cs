using LionFire.Instantiating;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets.Instantiating
{
    public class TemplateInstanceInfo<TTemplate, TParameters> : ITemplateInstance<TTemplate, TParameters>
        where TTemplate : class, ITemplateAsset
        where TParameters : class, ITemplateParameters
    {
        #region Parameters

        ITemplateParameters ITemplateInstance<TTemplate, TParameters>.Parameters { get { return parameters; } set { parameters = (TParameters)value; } }
        public TParameters Parameters
        {
            get => parameters;
            set
            {
                if (parameters != null) throw new AlreadyException("Can only be set once");
                parameters = value;
            }
        }
        private TParameters parameters;

        #endregion

        #region Template

        //ITemplateAsset ITemplateAssetInstance.TemplateAsset { get { return Template; } set { Template = (TTemplate)value; } }
        public TTemplate Template
        {
            get => template;
            set
            {
                if (ReferenceEquals(value, template)) return;
                if (template != null) throw new NotSupportedException("Can only be set once");
                template = value;
            }
        }
        private TTemplate template;

        #endregion
    }

    public class TemplateInstanceInfo<TTemplate> : ITemplateInstance<TTemplate, EmptyTemplateParameters>
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
