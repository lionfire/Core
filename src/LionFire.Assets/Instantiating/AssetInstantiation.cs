#define AssetInstantiation
using LionFire.Assets;
using LionFire.Copying;
using LionFire.Instantiating;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Referencing;
//using LionFire.ObjectBus;
using LionFire.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LionFire.Assets
{

    // OLD - was this a new one I started after porting from legacy to core?
    //public class AssetInstantiation : Instantiation, IInstantiation, IHasTemplateAsset
    ////where InstanceType : ITemplateAssetInstance, new()
    //{

    //    public AssetInstantiation(ITemplateParameters pObj)
    //    {
    //        Parameters = pObj;
    //        TemplateAsset = pObj.TemplateAsset;
    //    }
    //}


#if AssetInstantiation // Don't think this is needed?  OLD

    /// <summary>
    /// Represents the intent to instantiate an object from a template, and optionally template parameters and
    /// optionally state as understood by the instance.
    /// REVIEW - should this have anything to do with IAssetInstantiation?  If so, maybe rename to AssetInstantiation
    /// </summary>
    [LionSerializable(SerializeMethod.ByValue)] // REVIEW
    public class AssetInstantiation : IAssetInstantiation, IParented
    // , IParentedTemplateParameters TODO
    {

        #region Ontology - TODO - both Key and Pid???

        #region Parent

        #region Parent

        [Ignore]
        public object Parent { get; set; }

        #endregion

        [SerializeDefaultValue(false)]
        public string ParentKey { get; set; }

        #endregion

        #region Key

        // object IKeyed<string>.Key { get { return Key; } }

        [SerializeDefaultValue(false)]
        public string Key
        {
            get => key;
            set
            {
                if (key == value) return;
                if (key != default(string)) throw new NotSupportedException("Key can only be set once.");
                key = value;
            }
        }
        private string key;

        #endregion

        #region Pid

        [DefaultValue(0)]
        [SerializeDefaultValue(false)]
        public short Pid
        {
            get => pid;
            set
            {
                if (pid == value) return;
                if (value != 0 && pid != 0) throw new NotSupportedException("Pid can only be set once (unless first set back to default).");
                pid = value;
            }
        }
        private short pid = 0;

        public IPidRoot PidRoot => this.ParentOfType<IPidRoot>();

        public bool HasPid => pid != 0;

        public void EnsureHasPid()
        {
            if (!TryEnsureHasPid()) throw new Exception("Failed to get Pid: " + this.ToStringSafe());
        }

        public bool TryEnsureHasPid()
        {
            if (Pid == 0)
            {
                IPidRoot pidRoot = PidRoot;
                if (pidRoot != null)
                {
                    Pid = pidRoot.KeyKeeper.GetNextKey();
                    return Pid != 0;
                }
                else { return false; }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// OverlayTargets: by default, the template's IInstantiation tree
        /// </summary>
        public virtual IEnumerable<IEnumerable<IInstantiation>> OverlayTargets
        {
            get
            {
                throw new NotImplementedException("TOPORT");
                //if (TemplateAsset != null && TemplateAsset.Object != null)
                //{
                //    var hierarchicalTemplate = TemplateAsset.Object as IHierarchicalTemplate;
                //    if (hierarchicalTemplate != null && hierarchicalTemplate.Children != null)
                //    {
                //        yield return (IEnumerable<Instantiation>)hierarchicalTemplate.Children;
                //        //foreach (var child in hierarchicalTemplate.Children)
                //        //{
                //        //    yield return child;
                //        //}
                //    }
                //}
                ////var templateOverlayable = this.Parent as ITemplateOverlayable;
                ////if (templateOverlayable != null)
                ////{
                ////    foreach (var target in templateOverlayable.OverlayTargets) { yield return target; }
                ////}
                //yield break;
            }
        }

        #region Construction

        public AssetInstantiation()
        {
        }

        public AssetInstantiation(IPrototype prototype)
        {
            Prototype = prototype;
        }

        //public Instantiation(AssetReference<ITemplate> assetPath)
        //    : this()
        //{
        //    Template = assetPath;
        //}

        public AssetInstantiation(RH
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
<ITemplateAsset>
#endif
                             hTemplate)
        //            : this()
        {
            //Log.Info("ZX Instantiation.ctor");
            //Log.Info("ZX Instantiation.ctor " + (hTemplate == null ? "NULL" : hTemplate.Reference.ToString()));

            this.TemplateAsset = hTemplate;

            //Log.Info("ZX Instantiation.ctor end " + (hTemplate == null ? "NULL" : hTemplate.Reference.ToString()));

        }

        public AssetInstantiation(ITemplateAsset template)
        //       : this()
        {
            //Template = new AssetReference<ITemplate>(template);
            TemplateAsset = new HAsset
#if !AOT
                <ITemplateAsset>
#endif
                    (template);
        }

        // REVIEW - Make sure HAsset with interface types is documented somewhere and I understand it
        public AssetInstantiation(string template, IParentedTemplateParameters parameters = null)
            : this((HAsset
#if !AOT
                    <ITemplateAsset>
#endif
                    )template)
        {
            this.Parameters = parameters;
        }
        public AssetInstantiation(ITemplateAsset template, IParentedTemplateParameters parameters = null)
            : this(template)
        {
            this.Parameters = parameters;
        }

        public AssetInstantiation(RH
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
<ITemplateAsset>
#endif
 assetPath, IParentedTemplateParameters parameters = null)
            : this(assetPath)
        {
            this.Parameters = parameters;
        }

        public AssetInstantiation(RH
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
<ITemplateAsset>
#endif
 assetPath, IParentedTemplateParameters parameters = null, object state = null)
            : this(assetPath, parameters)
        {
            this.State = state;
        }

        public AssetInstantiation(ITemplateAsset template, IParentedTemplateParameters parameters = null, object state = null)
            : this(template, parameters)
        {
            this.State = state;
        }

        #endregion

        #region Implicit Construction



#if false // EXPERIMENT
        public static implicit operator Instantiation(string assetPath)
        {
            return new Instantiation((HAsset
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

        #region Prototype

        [SerializeDefaultValue(false)]
        public ICloneable Prototype { get; set; }

        #endregion

        #region Template

        //public ITemplate TemplateObject
        //{
        //    get
        //    {
        //        if (Template != null && Template.Object != null)
        //        {
        //            return Template.Object;
        //        }
        //        //if (TemplateHandle != null)
        //        //{
        //        //    return TemplateHandle.Object;
        //        //}
        //        return null;
        //    }
        //}
        //#region TemplateHandle

        ////public IReadHandle<ITemplate> TemplateHandle
        ////{
        ////    get { return templateHandle; }
        ////    set
        ////    {
        ////        if (templateHandle == value) return;
        ////        if (templateHandle != default(IReadHandle<ITemplate>)) throw new NotSupportedException("TemplateHandle can only be set once.");
        ////        templateHandle = value;
        ////    }
        ////} private IReadHandle<ITemplate> templateHandle;

        //#endregion

        //public IReadHandle TemplateAH
        //{
        //    get;
        //    set;
        //}

        #region Template

        [Ignore]
        public ITemplate Template
        {
            get => TemplateAsset?.Object;
            set => throw new NotSupportedException("To set the Template, use the TemplateAsset property.");
        }

        public object TemplateObj
        {
            get
            {
                if (templateAsset != null && templateAsset.GetType() == typeof(string))
                    throw new UnreachableCodeException("get_TemplateObj - got string: " + (string)(object)templateAsset);

                return templateAsset;
            }
        }


        [Assignment(AssignmentMode.Assign)]
        public RH<ITemplateAsset> TemplateAsset //#if !AOT && !UNITY // Unity used to crash with contravariant IReadHandle.  Does it still?
        {
            get
            {
#if RuntimeDebug
                if (templateAsset != null && templateAsset.GetType() == typeof(string))
                    throw new UnreachableCodeException("get_Template - got string: " + (string)(object)templateAsset);
#endif
                if (templateAsset == null && !object.ReferenceEquals(Parameters, this) && Parameters is IHasTemplateAsset hta)
                {
                    return hta.TemplateAsset;
                }
                return templateAsset;
            }
            set
            {
                if ((templateAsset == null && value == null) || (templateAsset != null && value != null && templateAsset.Equals(value))) return;
                //if (template != null) throw new NotSupportedException("Can only be set once.");
                if (templateAsset != null && value != null) throw new AlreadySetException();
                if (value != null && value.GetType() == typeof(string))
                    throw new UnreachableCodeException("set_Template - got string: " + (string)(object)value);
                templateAsset = value;

                //if (template != null && this.Key != null)
                //{
                //    this.TryGetOverlayParent();
                //}
            }
        }
        protected RH
#if !AOT && !UNITY // Unity crashes with contravariant IReadHandle
<ITemplateAsset>
#endif
 templateAsset;

        #endregion


        //public AssetReference<ITemplate> Template
        //{
        //    get { return template; }
        //    set
        //    {
        //        if (template == value) return;
        //        if (template != default(AssetReference<ITemplate>)) throw new NotSupportedException("Template can only be set once.");
        //        template = value;
        //    }
        //} private AssetReference<ITemplate> template;

        #endregion

        [SerializeDefaultValue(false)]
        public virtual ITemplateParameters Parameters { get; set; }

        #region Overlaying

        //public virtual ParameterOverlayMode OverlayMode { get { if (Parameters == null) return ParameterOverlayMode.None; return Parameters.OverlayMode; } set { throw new NotSupportedException("Cannot set OverlayMode on Instantiation."); } }

        //public virtual object OverlayParent
        //{
        //    get
        //    {
        //        if (Parameters == null)
        //        {
        //            return null;
        //        }
        //        return Parameters.OverlayParent;
        //    }
        //    set
        //    {
        //        if (Parameters == null) { throw new InvalidOperationException("Parameters == null"); }
        //        Parameters.OverlayParent = value;
        //    }
        //}

        #region OverlayMode

        [DefaultValue(ParameterOverlayMode.None)]
        [SerializeDefaultValue(false)]
        public ParameterOverlayMode OverlayMode
        {
            get => overlayMode;
            set => overlayMode = value;
        }
        private ParameterOverlayMode overlayMode = ParameterOverlayMode.None;

        #endregion

        #region OverlayParent

        [Ignore]
        [SerializeDefaultValue(false)]
        public object OverlayParent
        {
            get => _overlayParent;
            set => _overlayParent = value;
        }
        private object _overlayParent;

        #endregion

        #endregion

        [Ignore]
        public Func<AssetInstantiation, string> GetDefaultKey = GetDefaultKeyMethodDefault;

        public static Func<AssetInstantiation, string> GetDefaultKeyMethodDefault = ins =>
            {
                if (ins == null) return null;

                var provider = ins.Parameters as IDefaultInstanceKeyProvider;
                if (provider != null) { return provider.DefaultKey; }

                provider = ins.TemplateAsset as IDefaultInstanceKeyProvider;
                if (provider != null) { return provider.DefaultKey; }

                return null;
            };

        //[Ignore]
        [SerializeDefaultValue(false)]
        public virtual object State { get; set; }

        /// <summary>
        /// REVIEW - A way to avoid this is to break up Create into Construct and InitializeState
        /// </summary>
        /// <remarks>
        /// Valor: This is set by PortalSpawner.Spawn* methods.
        /// </remarks>
        [Ignore]
#if AOT
		public ActionObject InitializationMethod { get; set; }
#else
        public Action<object> InitializationMethod { get; set; }
#endif

        #region Children

        public bool HasChildren => children != null && children.Count > 0;

        //private static readonly List<Instance> defaultList = new List<Instance>();
        [SerializeDefaultValue(false)]
        //[JsonExSerializer.JsonExDefault(defaultList)]
        //[JsonExSerializer.JsonExDefaultValues(typeof(List<Instance>), )]
        [LionSerializable(SerializeMethod.ByValue)]
        public IInstantiationCollection Children
        {
            get { if (children == null) { Children = new InstantiationCollection(); } return children; }
            set
            {
                if (children == value) return;
                children = value;
                children.Parent = this;
            }
        }
        private IInstantiationCollection children;

        #endregion

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            if (children != null)
            {
                foreach (IAssetInstantiation child in this.children.Values)
                {
                    yield return child;

                    //foreach (IInstantiation result in child)
                    //{
                    //    yield return result;
                    //}
                }
            }
        }

        #endregion

        #region AllChildren

        public IEnumerable<IInstantiation> AllChildren
        {
            get
            {
                if (HasChildren)
                {
                    //foreach (LionFire.IEnumerableExtensions.Recursion child in (this.Children.SelectRecursive(x => ((IAssetInstantiation)x).GetChildrenEnumerable()))) // OLD AOT
                    foreach (var child in (this.Children.SelectRecursive(x => x.GetChildrenEnumerable()).Select(r => r.Item)))
                    {
                        yield return child;
                    }
                }
            }
        }

        #endregion

    }
#endif

}