using LionFire.DependencyInjection;
using LionFire.Resolvables;
using LionFire.Structures;
using System;

namespace LionFire.Referencing
{
    public abstract class TypedReferenceBase<TConcrete> : ReferenceBase<TConcrete>, ITypedReference
        where TConcrete : ReferenceBase<TConcrete>
    {

        #region Type

        [SetOnce]
        public Type Type
        {
            get => type;
            set
            {
                if (type == value) return;
                if (type != default) throw new AlreadySetException();
                type = value;
            }
        }
        private Type type;

        #endregion
    }

    public abstract class TypedReferenceBase<TConcrete, TReferenced> : ReferenceBase<TConcrete>, ITypedReference
        where TConcrete : ReferenceBase<TConcrete>
    {
        public Type Type => typeof(TReferenced);
    }

#if TOPORT
    public class AssetPathToVosReferenceResolver : IResolver<IResolvingReference, IReference>
    {
        public IReference Resolve<T>(T r) where T : IResolvingReference
        {
            new VosReference();
        }
    }
#endif

    public abstract class ResolvingTypedReferenceBase<TConcrete, TReferenced> : ReferenceBase<TConcrete>, ITypedReference, IResolvable
        where TConcrete : ReferenceBase<TConcrete>
    {
        public Type Type => typeof(TReferenced);

        public IReference ResolvedReference
        {
            get
            {
                if(resolvedReference == null)
                {
                    resolvedReference =((TConcrete)(object)this).Resolve<TConcrete, IReference>(); // HARDCAST
                }
                return resolvedReference;
            }
            protected set
            {
                resolvedReference = value;
            }
        }
        private IReference resolvedReference;

        public override string Scheme => resolvedReference.Scheme;
        public override string Host { get => resolvedReference.Host; set => throw new NotSupportedException(); }
        public override string Port { get => resolvedReference.Port; set => throw new NotSupportedException(); }
        public override string Key { get => resolvedReference.Key; protected set => throw new NotSupportedException(); }
        public override string Path { get => resolvedReference.Path; set => throw new NotSupportedException(); }
    }
}
