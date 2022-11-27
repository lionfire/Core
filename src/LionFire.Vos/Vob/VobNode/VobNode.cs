using LionFire.Collections;
using LionFire.Referencing;
using LionFire.Vos.Internals;
using System;
using System.Text;

namespace LionFire.Vos
{

    public class VobNode<TInterface> : IVobNode<TInterface>
        where TInterface : class
    {

        public IVob Vob { get; }

        public VobNode(Vob vob, Func<IVobNode, TInterface> factory)
        {
            Vob = vob;
            Value = factory(this);
        }

        public TInterface Value { get; }
        object IVobNode.Value => Value;

        public TInterface ParentNodeValue
        {
            get
            {
                IVob ancestor = Vob.Parent;
                TInterface parentNodeValue = null;
                while (ancestor != null && (parentNodeValue = ancestor.GetOwnRequired<TInterface>()) == null) ancestor = ancestor.Parent;

                return parentNodeValue;
            }
        }
        public IVobNode<TInterface> NextAncestor
        {
            get
            {
                IVob ancestor = Vob.Parent;
                VobNode<TInterface> parentVobNode = null;
                while (ancestor != null && (parentVobNode = ancestor.TryGetOwnVobNode<TInterface>()) == null) ancestor = ancestor.Parent;

                return parentVobNode;
            }
        }        
    }


#if truex
    /// <summary>
    /// VobNodes contain the data for heavier Vobs that have complex things such as mounts or other metadata
    /// </summary>
    public partial class VobNode : IVobNodeFacade
    {

    #region Identity

        public bool IsVobNode => true;
        public Vob Vob { get; }

    #endregion

    #region Relationships

    #region ParentVobNode

        public VobNode ParentVobNode
        {
            get
            {
                IVob ancestor = Vob.Parent;
                while (ancestor != null && ancestor.GetOwn<TInterface>() == null) ancestor = ancestor.Parent;

                return ancestor?.VobNode;
            }
        }
        protected event Action<VobNode, VobNode, VobNode> ParentVobNodeChangedForFromTo;

    #endregion

    #endregion

    #region Options

        public VobNodeOptions Options { get; set; }

    #endregion

    #region Construction

        public VobNode(Vob vob)
        {
            Vob = vob;
            VobDepth = LionPath.GetAbsolutePathDepth(vob.Path);
        }

    #endregion

    #region Derived

        internal int VobDepth { get; }

    #endregion
        
        public override string ToString() => $"{{VobNode {Vob.Path}}}";
    }
#endif
}
