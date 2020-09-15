using LionFire.Dependencies;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Resolvables;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

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
    public class AssetPathToVobReferenceResolver : IResolver<IResolvingReference, IReference>
    {
        public IReference Resolve<T>(T r) where T : IResolvingReference
        {
            new VobReference();
        }
    }
#endif

    // TODO: Document, maybe rename generic type names
    public abstract class ResolvingTypedReferenceBase<TConcrete, TReferenced> : ReferenceBase<TConcrete>, ITypedReference, IResolvable
        where TConcrete : ReferenceBase<TConcrete>
    {
        public Type Type => typeof(TReferenced);

        [Blocking(Alternative = nameof(ResolveReference))]
        public IReference ResolvedReference
        {
            get
            {
                if(resolvedReference == null)
                {
                    ResolveReference().Wait();
                }
                return resolvedReference;
            }
            protected set
            {
                resolvedReference = value;
            }
        }
        private IReference resolvedReference;

        public async Task ResolveReference()
        {
            if (resolvedReference == null)
            {
                resolvedReference = (await ((TConcrete)(object)this).Resolve<TConcrete, IReference>().ConfigureAwait(false)).Value; // HARDCAST
            }
        }

        public override string Scheme => resolvedReference.Scheme;
        //public override string Host { get => resolvedReference.Host; set => throw new NotSupportedException(); }
        //public override string Port { get => resolvedReference.Port; set => throw new NotSupportedException(); }
        public override string Key { get => resolvedReference.Key; protected set => throw new NotSupportedException(); }
        public override string Path { get => resolvedReference.Path; protected set => throw new NotSupportedException(); }
    }
}
