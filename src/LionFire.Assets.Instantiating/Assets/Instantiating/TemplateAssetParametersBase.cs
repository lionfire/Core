using LionFire.Instantiating;
using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Assets
{
    public abstract class TemplateAssetParametersBase<TTemplate> : TemplateParametersBase<TTemplate>
        //, IAssetInstantiation
        where TTemplate : ITemplateAsset
    {
        protected TemplateAssetParametersBase() { }
        protected TemplateAssetParametersBase(IReadHandleBase<TTemplate> template) : base(template) { }

        //public new IEnumerable<IAssetInstantiation> AllChildren => base.Children.OfType<IAssetInstantiation>(); // REVIEW - new
        //public IReadHandle<ITemplateAsset> TemplateAsset { get => (IReadHandle<ITemplateAsset>)Template; set => Template = (IReadHandleBase<TTemplate>)value; }

    }

    //    public abstract class TemplateAssetParametersBase<TTemplate> : InstantiationBase<TTemplate>, ITemplateParameters<TTemplate>
    //    //, IAssetInstantiation
    //    where TTemplate : ITemplateAsset
    //{
    //    // REVIEW - all this casting
    //    IReadHandleBase<ITemplate> ITemplateParameters.Template => (IReadHandleBase<ITemplate>)Template;

    //    //public new IReadHandleBase<TTemplate> Template { get; set; } // REVIEW - new
    //    //public new R<TTemplate> Template { get; set; } // REVIEW - new
    //    //public TTemplate OTemplate => Template.Value; // REVIEW

    //    public IReadHandle<ITemplateAsset> TemplateAsset { get => (IReadHandle<ITemplateAsset>)Template; set => Template = (IReadHandleBase<TTemplate>)value; }

    //    public override string Key { get => Template?.Reference.Key; set => throw new NotSupportedException(); }

    //    public new IEnumerable<IAssetInstantiation> AllChildren => base.Children.OfType<IAssetInstantiation>(); // REVIEW - new


    //    protected TemplateAssetParametersBase() { }
    //    protected TemplateAssetParametersBase(IReadHandleBase<TTemplate> template) { Template = template; }
    //}

    public abstract class TemplateAssetParametersBase<TTemplate, TInstance> : TemplateAssetParametersBase<TTemplate>, ITemplateParameters<TTemplate, TInstance>
        where TTemplate : ITemplateAsset<TTemplate, TInstance>

    {
        public TemplateAssetParametersBase() { }
        public TemplateAssetParametersBase(IReadHandleBase<TTemplate> template) : base(template) { }

        //IReadHandleBase<ITemplate> ITemplateParameters.Template => (IReadHandleBase<ITemplate>)Template;
        //public IReadHandleBase<TTemplate> Template { get; set; }

        //public string Key => Template?.Reference.Key;

        //public abstract object State { get; set; }
    }

}
