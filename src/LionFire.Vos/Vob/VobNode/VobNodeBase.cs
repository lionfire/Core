using System;

namespace LionFire.Vos
{
    public abstract class VobNodeBase<TConcrete> : IVobNode<TConcrete>
        where TConcrete : class
    {
        protected VobNodeBase(Vob vob) { this.Vob = vob; }

        public Vob Vob { get; }

        public TConcrete Value => this;
        object IVobNode.Value => Value;

        public IVobNode<TConcrete> ParentVobNode => Vob.Parent?.TryGetNextVobNode<TConcrete>(startAtThis: false).Value;
        public TConcrete ParentValue => Vob.Parent?.GetNext<TConcrete>(skipOwn: true);
    }
}

