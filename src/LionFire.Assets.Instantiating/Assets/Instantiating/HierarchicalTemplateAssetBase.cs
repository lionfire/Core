using LionFire.Instantiating;
using LionFire.Ontology;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets.Instantiating
{
    // PORTINGGUIDE - HierarchicalTemplateBase > HierarchicalTemplateAssetBase
    public abstract class HierarchicalTemplateAssetBase<TTemplate, TInstance, TInstantiation> : TemplateAssetBase<TTemplate, TInstance, TInstantiation>, IHierarchicalTemplate
         , IParented
         where TTemplate : class, ITemplateAsset<TTemplate, TInstance>
         where TInstance : class, ITemplateAssetInstance, new()
         where TInstantiation : IInstantiation, ITemplateParameters<TTemplate, TInstance>, new()
    {
        #region Parent

        [Ignore]
        public object Parent { get; set; }
        ////public object Parent { get { return this.Parent; } set { this.Parent = (IHierarchicalTemplate)value; } }
        ////public IHierarchicalTemplate Parent { get; set; }

        #endregion

        #region Construction

        protected HierarchicalTemplateAssetBase()
            : base()
        {
        }
        protected HierarchicalTemplateAssetBase(AssetReference<TTemplate> reference) : base(reference)
        {
        }
        //		public HierarchicalTemplateAssetBase(HAsset asset)
        //			: base(asset)
        //		{
        //		}
        //#if !AOT

#if IRWAssetAware
        public HierarchicalTemplateAssetBase(RWAsset<TTemplate> asset)
            : base(asset)
        {
        }
#endif
        //#endif

        #endregion

        #region Children

        public bool HasChildren { get { return children != null && children.Count > 0; } }
        [SerializeDefaultValue(false)]
        public IInstantiationCollection Children
        {
            get
            {
                return children;
            }
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

        //IEnumerable<IInstantiation> IHierarchicalTemplate.Children => Children;

        private IInstantiationCollection children;

        #endregion

        #region IEnumerable Implementation

//#if !AOT
//        IEnumerator<IAssetInstantiation> IEnumerable<IAssetInstantiation>.GetEnumerator()
//        {
//            if (Children == null)
//            {
//                yield break;
//            }
//            else
//            {
//                foreach (var x in Children.Values) yield return x;
//            }
//        }
//#endif

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    if (Children == null)
        //    {
        //        yield break;
        //    }
        //    else
        //    {
        //        foreach (var x in ((IEnumerable)Children.Values)) yield return x;
        //    }
        //}

        #endregion

    }



}
