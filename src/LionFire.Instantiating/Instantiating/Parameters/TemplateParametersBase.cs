using LionFire.Instantiating;
using LionFire.Ontology;
using LionFire.Persistence;
using System;
using System.Collections.Generic;

namespace LionFire.Assets
{
    public abstract class TemplateParametersBase<TTemplate, TInstance, TState> : InstantiationBase<TTemplate, TState>, ITemplateParameters<TTemplate, TInstance>
        where TTemplate : ITemplate<TInstance>
    {
        protected TemplateParametersBase() { }
        protected TemplateParametersBase(IReadHandleBase<TTemplate> template) : base(template) { }

    }

    public abstract class TemplateParametersBase<TTemplate, TInstance> : TemplateParametersBase<TTemplate, TInstance, object>
        where TTemplate : ITemplate<TInstance>
    {
        protected TemplateParametersBase() { }
        protected TemplateParametersBase(IReadHandleBase<TTemplate> template) : base(template) { }
    }

    public abstract class TemplateParametersBase<TTemplate> : InstantiationBase<TTemplate>, ITemplateParameters<TTemplate>
          where TTemplate : ITemplate
    {
        //public override string Key { get => RTemplate?.Reference.Key; set => throw new NotSupportedException(); }

        #region Key

        [SetOnce]
        public override string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException();
                key = value;
            }
        }
        private string key;

        #endregion



        protected TemplateParametersBase() { }
        protected TemplateParametersBase(IReadHandleBase<TTemplate> template) { RTemplate = template; }
        protected TemplateParametersBase(TTemplate template) { Template = template; }
    }

    public abstract class ParentedTemplateParametersBase<TTemplate> : TemplateParametersBase<TTemplate>, IParentedTemplateParameters<TTemplate>
        //, IParented
        where TTemplate : ITemplate
    {
        protected ParentedTemplateParametersBase() { }
        protected ParentedTemplateParametersBase(IReadHandleBase<TTemplate> template, string parentKey) { RTemplate = template; ParentKey = parentKey; }



        //#region Parent

        //[SetOnce]
        //public object Parent
        //{
        //    get => parent;
        //    set
        //    {
        //        if (parent == value) return;
        //        if (parent != default) throw new AlreadySetException();
        //        parent = value;
        //    }
        //}
        //private object parent;

        //#endregion



    }
}
