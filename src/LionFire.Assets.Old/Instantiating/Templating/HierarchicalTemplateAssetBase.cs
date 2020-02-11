using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Assets;
using System.Collections;
using LionFire.Structures;
using LionFire.Ontology;

namespace LionFire.Instantiating
{
    //[Obsolete] OLD - keep for a while?
    //public class HierarchicalTemplateBase<InstanceType> : TemplateBase<InstanceType>, IHierarchicalTemplate
    //    where InstanceType : ITemplateInstance, new()
    //{
    //    #region Construction

    //    public HierarchicalTemplateBase()
    //    {
    //        Children = new InstantiationCollection();
    //    }
    //    public HierarchicalTemplateBase(AssetID id)
    //        : this()
    //    {
    //        this.ID = id;
    //        this.ID.Package = AssetContext.Current.DefaultCreatePackage;
    //    }

    //    #endregion

    //    #region Children

    //    public bool HasChildren { get { return children != null && children.Count > 0; } }
    //    [SerializeDefaultValue(false)]
    //    public InstantiationCollection Children
    //    {
    //        get {
    //            return children; }
    //        set { children = value; }
    //    }
    //    private InstantiationCollection children;

    //    #endregion

    //    //#region Templates

    //    //public bool HasTemplates { get { return templates != null && templates.Count > 0; } }
    //    //[SerializeDefaultValue(false)]
    //    //public IList<ITemplate> Templates
    //    //{
    //    //    get
    //    //    {
    //    //        return templates;
    //    //    }
    //    //    set { templates = value; }
    //    //}
    //    //private IList<ITemplate> templates;

    //    //#endregion

    //    //public IEnumerator<IInstantiation> GetEnumerator()
    //    //{
    //    //    if (Children == null) return Enumerable.Empty<IInstantiation>().GetEnumerator();
    //    //    return Children.GetEnumerator();
    //    //}
    //    // IEnumerator IEnumerable.GetEnumerator()
    //    //{
    //    //    if (Children == null) return Enumerable.Empty<IInstantiation>().GetEnumerator();
    //    //    return Children.GetEnumerator();
    //    //}
    //}

    //[Obsolete] OLD - keep for a while?
    //public class HierarchicalTemplateBase<InstanceType, InstantiationType_> : HierarchicalTemplateBase<InstanceType>
    //    where InstanceType : ITemplateInstance, new()
    //    where InstantiationType_ : Instantiation, new()
    //{
    //    #region Construction

    //    public HierarchicalTemplateBase()
    //        : base()
    //    {
    //    }
    //    public HierarchicalTemplateBase(AssetID id)
    //        : base(id)
    //    {
    //    }

    //    #endregion

    //    public override Type InstantiationType { get { return typeof(InstantiationType_); } }
    //}

    // RENAME to HierarchicalTemplateAssetBase
    // TODO REVIEW - Can this have IInstantiations, or just IAssetInstantiations?  IEnumerable<IAssetInstantiation> and Children support different types.
    public abstract class HierarchicalTemplateAssetBase<TemplateType, InstanceType, InstantiationType_> : TemplateAssetBase<TemplateType, InstanceType, InstantiationType_>
        , IHierarchicalTemplate
        , IEnumerable<IAssetInstantiation>
        , IParented
        where TemplateType : class, ITemplateAsset
        where InstanceType : class, ITemplateAssetInstance, new()
        where InstantiationType_ : Instantiation, IAssetInstantiation, new()
    {
        #region Parent

        [Ignore]
        public object Parent { get; set; }
        ////public object Parent { get { return this.Parent; } set { this.Parent = (IHierarchicalTemplate)value; } }
        ////public IHierarchicalTemplate Parent { get; set; }

        #endregion

        #region Construction

        public HierarchicalTemplateAssetBase()
            : base()
        {
        }
        //		public HierarchicalTemplateBase(HAsset asset)
        //			: base(asset)
        //		{
        //		}
        //#if !AOT
        public HierarchicalTemplateAssetBase(HAsset<TemplateType> asset)
            : base(asset)
        {
        }
        //#endif

        #endregion

        #region Children

        //List<IInstantiation> IHierarchicalTemplate.Children
        //{
        //    get => Children.AllItems;
        //    set => Children.AllItems = value;
        //}

        public bool HasChildren { get { return children != null && children.Count > 0; } }

        IEnumerable<IInstantiation> IHierarchicalTemplate.Children => Children;
        [SerializeDefaultValue(false)]
        public IInstantiationCollection Children
        {
            get => children;
            set
            {
                if (children == value) return;
                children = value;
                if (children != null)
                {
                    children.Parent = this;
                }
            }
        }
        private IInstantiationCollection children;

        #endregion

        #region IEnumerable Implementation

#if !AOT
        IEnumerator<IAssetInstantiation> IEnumerable<IAssetInstantiation>.GetEnumerator()
        {
            if (Children == null)
            {
                yield break;
            }
            else
            {
                foreach (var x in Children.Values.OfType<IAssetInstantiation>()) yield return x;
            }
        }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Children == null)
            {
                yield break;
            }
            else
            {
                foreach (var x in ((IEnumerable)Children.Values)) yield return x;
            }
        }

        #endregion

    }



}
