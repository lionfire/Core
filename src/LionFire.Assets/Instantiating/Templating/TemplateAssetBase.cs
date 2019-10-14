using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Instantiating;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{

#if NEW // TODO
    public class TemplateBaseBase<TemplateType, InstanceType_, InstantiationType_> : AssetBase<TemplateType>,
#if !AOT
 ITemplate<InstanceType_>,
#endif
 ITemplate
        where TemplateType : class, ITemplate
        where InstanceType_ : class, ITemplateInstance
        where InstantiationType_ : IInstantiation, new()
    {
    #region Construction

        public TemplateBaseBase() { }
        //		public TemplateBaseBase(HAsset hAsset) : base(hAsset) { }
//#if !AOT
        public TemplateBaseBase(HAsset<TemplateType> hAsset) : base(hAsset) { }
//#endif

    #endregion
    }

    public class AbstractTemplateBase<TemplateType, InstanceType_, InstantiationType_> : TemplateBaseBase<TemplateType, InstanceType_, InstantiationType_>
        where TemplateType : class, ITemplate
        where InstanceType_ : class, ITemplateInstance
        where InstantiationType_ : IInstantiation, new()
    {
    #region Construction

        public AbstractTemplateBase() { }
//#if !AOT
        public AbstractTemplateBase(HAsset<TemplateType> hAsset) : base(hAsset) { }
//#endif

    #endregion
    }
#endif

#if TOPORT
    [Asset(IsAbstract = true)]
    public class TemplateAssetBase<TemplateType, InstanceType_> : TemplateAssetBase<TemplateType, InstanceType_, Instantiation>
        where TemplateType : class, ITemplateAsset
        where InstanceType_ : class, ITemplateAssetInstance//, new() // AOT - added class... if this works, take notice!
    {
    #region Construction

        public TemplateAssetBase() { }
        //		public TemplateBase(HAsset hAsset) : base(hAsset) { }
        //#if !AOT
        public TemplateAssetBase(HAsset<TemplateType> hAsset) : base(hAsset) { }
        //#endif

    #endregion
    }
#endif


    //    public class TemplateBase<TemplateType, InstanceType_, InstantiationType_> :
    //#if !AOT
    //    ITemplate<InstanceType_>,
    //#endif
    //	ITemplate
    //        where TemplateType : class, ITemplate
    //        where InstanceType_ : class, ITemplateAssetInstance, new() // AOT - added class... if this works, take notice!
    //        where InstantiationType_ : IInstantiation, new()
    //    {
    //        #region Construction

    //        public TemplateBase() { }

    //        #endregion

    //        #region Instantiation

    //        public Type InstanceType { get { return typeof(InstanceType_); } }

    //        // FUTURE ENH: IHasDynamicInstantiationType, TemplateExtensions.CreateInstantiation
    //        public Type InstantiationType { get { return typeof(InstantiationType_); } }

    //        IInstantiation ITemplate.CreateInstantiation() { return this.CreateInstantiation(); }
    //        //IAssetInstantiation ITemplateAsset.CreateInstantiation() { return this.CreateInstantiation(); }

    //        // REFACTOR: Use a static class to instantiate and set the template, if the instance has IHasTemplate
    //        public InstantiationType_ CreateInstantiation()
    //        {
    //            InstantiationType_ instantiation = (InstantiationType_)Activator.CreateInstance(InstantiationType);
    //            instantiation.Template = this; 
    //            return instantiation;
    //        }

    //#endregion

    //#region Instance

    //        ITemplateAssetInstance ITemplate.ConstructInstance()
    //        {

    //#if AOT && true // AOTTEMP
    //			object obj = ConstructInstance();
    //			if(obj.GetType() == typeof(string))
    //				l.Fatal("[RUNTIME ERROR] TemplateBase.ConstructInstance was supposed to return ITemplateInstance but returned string instead");
    //			return obj as ITemplateInstance;

    //#else
    //#if AOT
    //			(ITemplateInstance)(object) // REVIEW Not sure this helps but something weird is going on
    //#endif

    //            return ConstructInstance();
    //#endif
    //        }

    //        public InstanceType_ ConstructInstance()
    //        {
    //            InstanceType_ instance = (InstanceType_)Activator.CreateInstance(typeof(InstanceType_));

    //            //			InstanceType_ instance = new InstanceType_();
    //            //instance.Template = this;
    //            return instance;
    //        }

    //#endregion

    //#region Misc

    //        private static ILogger l = Log.Get();

    //#endregion
    //    }

    // TODO: Try removing new() constraint on InstanceType_ and set up a DI mechanism for activating InstanceTypes at runtime.
    [Asset(IsAbstract = true)]
    public class TemplateAssetBase<TemplateType, InstanceType_, InstantiationType_> : AssetBase<TemplateType>,
#if !AOT
    ITemplateAsset<InstanceType_>,
#endif
    ITemplateAsset
        where TemplateType : class, ITemplateAsset
        where InstanceType_ : class, ITemplateAssetInstance//, new() // AOT - added class... if this works, take notice!
        where InstantiationType_ : IInstantiation, new()
    {
        #region Construction

        public TemplateAssetBase() { }
        //		public TemplateBase(HAsset hAsset) : base(hAsset) { }
        //#if !AOT
        public TemplateAssetBase(HAsset<TemplateType> hAsset) : base(hAsset) { }
        //#endif

        #endregion

        #region Instantiation

        public Type InstanceType { get { return typeof(InstanceType_); } }

        public Type InstantiationType { get { return typeof(InstantiationType_); } }

        //IInstantiation ITemplate.CreateInstantiation() { return this.CreateInstantiation(); } TOPORT ?
        //IAssetInstantiation ITemplateAsset.CreateAssetInstantiation() { return this.CreateInstantiation(); }

        public InstantiationType_ CreateInstantiation()
        {
            var hAsset = this.HAsset;
            if (hAsset == null)
            {
                hAsset = new HAsset
                    //					#if !AOT
                    <TemplateType>
                        //#endif
                        ((TemplateType)(object)this
#if false
						 , assetType: typeof(TemplateType)
#endif
                         );
                //throw new InvalidOperationException("Cannot call CreateInstantiation on a template that is not referencable.  Set the name first.");

            }
            else
            {
                if (!hAsset.HasObject)
                {
                    hAsset.Object = (TemplateType)(object)this;
                }
            }
            //l.Fatal("hAsset.HasObject " +  hAsset.HasObject + " path: " + hAsset.AssetPath);

            var instantiation = (InstantiationType_)Activator.CreateInstance(InstantiationType) ;
            if(instantiation is IAssetInstantiation assetInstantiation)
            {
                assetInstantiation.TemplateAsset = hAsset;
            }
#if SanityChecks
			if(instantiation.TemplateAsset == null){throw new UnreachableCodeException("instantiation.Template == null");}
#endif
            return instantiation;
        }

        #endregion

        #region Instance

//        ITemplateAssetInstance ITemplate.ConstructInstance()
//        {

//#if AOT && true // AOTTEMP
//			object obj = ConstructInstance();
//			if(obj.GetType() == typeof(string))
//				l.Fatal("[RUNTIME ERROR] TemplateBase.ConstructInstance was supposed to return ITemplateInstance but returned string instead");
//			return obj as ITemplateInstance;

//#else
//#if AOT
//			(ITemplateInstance)(object) // REVIEW Not sure this helps but something weird is going on
//#endif

//            return ConstructInstance();
//#endif
//        }

        public InstanceType_ ConstructInstance()
        {
            InstanceType_ instance = (InstanceType_)Activator.CreateInstance(typeof(InstanceType_));

            //			InstanceType_ instance = new InstanceType_();
            //instance.Template = this;
            return instance;
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

    // [Obsolete]
    //public class TemplateBase<InstanceType> : AssetBase, ITemplate<InstanceType>
    //    where InstanceType : ITemplateInstance, new()
    //{
    //    #region Construction

    //    public TemplateBase()
    //    {
    //    }
    //    public TemplateBase(AssetID id) : this() { this.ID = id; }

    //    ITemplateInstance ITemplate.ConstructInstance() { return ConstructInstance(); }
    //    public InstanceType ConstructInstance()
    //    {
    //        return new InstanceType();
    //    }

    //    #endregion

    //    #region Instantiation

    //    public virtual Type InstantiationType { get { return typeof(Instantiation); } }

    //    //IInstantiation ITemplate.CreateInstantiation()
    //    //{
    //    //    return CreateInstantiation();
    //    //}

    //    //public IInstantiation<InstanceType> CreateInstantiation()
    //    public IInstantiation CreateInstantiation()
    //    {
    //        //IInstantiation<InstanceType> instantiation = (IInstantiation<InstanceType>)Activator.CreateInstance(InstantiationType);
    //        IInstantiation instantiation = (IInstantiation)Activator.CreateInstance(InstantiationType);
    //        //instantiation.Template = new HRObject<ITemplate>(this);
    //        instantiation.Template = new HAsset<ITemplate>(this);
    //        return instantiation;
    //    }

    //    #endregion
    //}

    // [Obsolete]
    //public class TemplateBase<InstanceType, InstantiationType_> : TemplateBase<InstanceType>
    //    where InstanceType : ITemplateInstance, new()
    //    where InstantiationType_ : Instantiation, new()
    //{
    //    #region Construction

    //    public TemplateBase()
    //        : base()
    //    {
    //    }
    //    public TemplateBase(AssetID id)
    //        : base(id)
    //    {
    //    }

    //    #endregion

    //    public override Type InstantiationType { get { return typeof(InstantiationType_); } }
    //}

    //public class HRObject<T> : IReadHandle<T> // MOVE
    //    where T:class
    //{
    //    public HRObject(T obj)
    //    {
    //        this.obj = obj;
    //    }

    //    public bool HasObject { get { return obj != null; } }
    //    public T Object
    //    {
    //        get { return obj; }
    //    } private T obj;

    //    public T ObjectField
    //    {
    //        get { return obj; }
    //    }
    //}
}
