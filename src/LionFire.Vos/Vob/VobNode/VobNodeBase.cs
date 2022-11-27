using LionFire.Vos.Internals;
using System;

namespace LionFire.Vos;

public abstract class VobNodeBase<TConcrete> : IVobNode<TConcrete>
    where TConcrete : VobNodeBase<TConcrete>
{
    protected VobNodeBase(Vob vob) { this.Vob = vob; }

    IVob IVobNode.Vob => Vob;
    public Vob Vob { get; }

    public TConcrete Value => (TConcrete)this;
    object IVobNode.Value => Value;

    //public IVobNode<TConcrete> NextAncestor => Vob.Parent.TryGetNextVobNode<TConcrete>();
    public TConcrete ParentValue => Vob.Parent?.Acquire<TConcrete>(minDepth: 1);
}

