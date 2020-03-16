using System;
using System.Text;
using LionFire.Collections;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Instantiating
{
    public class Instantiation<TTemplate, TInstance> : Instantiation<TTemplate>, ITemplateParameters<TTemplate, TInstance>
       where TTemplate : ITemplate<TInstance>
    {
    }

    public class Instantiation<TTemplate> : InstantiationBase<TTemplate>, ITemplateParameters<TTemplate>
       where TTemplate : ITemplate
    {
        #region Key

        [SetOnce]
        [SerializeDefaultValue(false)]
        public override string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException("Key can only be set once.");
                key = value;
            }
        }
        private string key;

        #endregion

        public Instantiation() { }

        public Instantiation(IReadHandleBase<TTemplate> hTemplate)
        {
            this.RTemplate = hTemplate;
        }

        public Instantiation(TTemplate template)
        //       : this()
        {
            RTemplate = new ObjectHandle<TTemplate>(template);
        }


        // REVIEW - Make sure H with interface types is documented somewhere and I understand it
        public Instantiation(string template, IParentedTemplateParameters parameters = null)
        //: this((H<TTemplate>)template)
        {
            throw new NotImplementedException("TOPORT: cast template from string to Handle");
            //this.Parameters = parameters;
        }
        public Instantiation(TTemplate template, IParentedTemplateParameters<TTemplate> parameters = null)
            : this(template)
        {
            this.Parameters = parameters;
        }

        public Instantiation(IReadHandleBase<TTemplate> assetPath, IParentedTemplateParameters<TTemplate> parameters = null)
            : this(assetPath)
        {
            this.Parameters = parameters;
        }

        //#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
        public Instantiation(IReadHandleBase<TTemplate> assetPath, IParentedTemplateParameters<TTemplate> parameters = null, object state = null)
            : this(assetPath, parameters)
        {
            this.State = state;
        }

        public Instantiation(TTemplate template, IParentedTemplateParameters<TTemplate> parameters = null, object state = null)
            : this(template, parameters)
        {
            this.State = state;
        }
    }

    public class Instantiation : InstantiationBase<ITemplate> // REFACTOR - change base to Instantiation<ITemplate>
    {
        #region Key

        [SetOnce]
        [SerializeDefaultValue(false)]
        public override string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default) throw new AlreadySetException("Key can only be set once.");
                key = value;
            }
        }
        private string key;

        #endregion

        #region Construction

        public Instantiation()
        {
        }

        public Instantiation(IPrototype prototype)
        {
            Prototype = prototype;
        }

        //public Instantiation(Reference<ITemplate> assetPath)
        //    : this()
        //{
        //    Template = assetPath;
        //}

        public Instantiation(IReadHandleBase<ITemplate> hTemplate)
        {
            //Log.Info("ZX Instantiation.ctor");
            //Log.Info("ZX Instantiation.ctor " + (hTemplate == null ? "NULL" : hTemplate.Reference.ToString()));

            this.RTemplate = hTemplate;

            //Log.Info("ZX Instantiation.ctor end " + (hTemplate == null ? "NULL" : hTemplate.Reference.ToString()));

        }

        public Instantiation(ITemplate template)
        //       : this()
        {
            //Template = new Reference<ITemplate>(template);
            RTemplate = new ObjectHandle<ITemplate>(template);
        }


        // REVIEW - Make sure H with interface types is documented somewhere and I understand it
        public Instantiation(string template, IParentedTemplateParameters parameters = null)
        //: this((H<ITemplate>)template)
        {
            throw new NotImplementedException("TOPORT: cast template from string to Handle");
            //this.Parameters = parameters;
        }
        public Instantiation(ITemplate template, IParentedTemplateParameters<ITemplate> parameters = null)
            : this(template)
        {
            this.Parameters = parameters;
        }

        public Instantiation(IReadHandleBase<ITemplate> assetPath, IParentedTemplateParameters<ITemplate> parameters = null)
            : this(assetPath)
        {
            this.Parameters = parameters;
        }

        //#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
        public Instantiation(IReadHandleBase<ITemplate> assetPath, IParentedTemplateParameters<ITemplate> parameters = null, object state = null)
            : this(assetPath, parameters)
        {
            this.State = state;
        }

        public Instantiation(ITemplate template, IParentedTemplateParameters<ITemplate> parameters = null, object state = null)
            : this(template, parameters)
        {
            this.State = state;
        }


        #region Implicit Construction

#if false // EXPERIMENT
        public static implicit operator Instantiation(string assetPath)
        {
            return new Instantiation((H
#if !AOT
			                          <ITemplate>
#endif
			                          )assetPath);
        }
#endif

        //public static implicit operator Instantiation(IReadHandle<ITemplate> handle)
        //{
        //    return new Instantiation((IReadHandle<ITemplate>)handle);
        //}

        #endregion

        #endregion

    }

}
