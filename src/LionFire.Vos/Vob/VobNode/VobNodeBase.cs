using LionFire.Vos.Internals;
using System;

namespace LionFire.Vos
{
    public abstract class VobNodeBase<TConcrete> : IVobNode<TConcrete>
        where TConcrete : VobNodeBase<TConcrete>
    {
        protected VobNodeBase(Vob vob) { this.Vob = vob; }

        public Vob Vob { get; }

        public TConcrete Value => (TConcrete)this;
        object IVobNode.Value => Value;

        public IVobNode<TConcrete> ParentVobNode => Vob.Parent?.TryGetNextVobNode<TConcrete>(skipOwn: true);
        public TConcrete ParentValue => Vob.Parent?.GetNext<TConcrete>(skipOwn: true);
    }
}

