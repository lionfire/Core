using LionFire.Instantiating;
using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Assets
{

    public abstract class TemplateAssetParametersBase<TTemplate> : InstantiationBase, ITemplateParameters<TTemplate>, IAssetInstantiation
        where TTemplate : ITemplate // REVIEW - remove class constraint?
    {
        // REVIEW - all this casting
        IReadHandleBase<ITemplate> ITemplateParameters.Template => (IReadHandleBase<ITemplate>)Template;
        public IReadHandleBase<TTemplate> Template { get; set; }
        public IReadHandle<ITemplateAsset> TemplateAsset { get => (IReadHandle<ITemplateAsset>)Template; set => Template = (IReadHandleBase<TTemplate>)value; }

        public override string Key { get => Template?.Reference.Key; set => throw new NotSupportedException(); }

        public IEnumerable<IAssetInstantiation> AllChildren => base.Children.OfType<IAssetInstantiation>();

    }

    public abstract class TemplateAssetParametersBase<TTemplate, TInstance> : TemplateAssetParametersBase<TTemplate>, ITemplateParameters<TTemplate, TInstance>
        where TTemplate : ITemplate<TInstance> // REVIEW - remove class constraint?

    {
        //IReadHandleBase<ITemplate> ITemplateParameters.Template => (IReadHandleBase<ITemplate>)Template;
        //public IReadHandleBase<TTemplate> Template { get; set; }

        //public string Key => Template?.Reference.Key;

        //public abstract object State { get; set; }
    }

}
