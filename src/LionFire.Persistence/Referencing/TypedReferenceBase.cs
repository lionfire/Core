using LionFire.Data.Gets;
using LionFire.Dependencies;
using LionFire.ExtensionMethods.Poco.Resolvables;
using LionFire.Structures;
using System;
using System.Threading.Tasks;
using LionFire.Data.Gets;

namespace LionFire.Referencing
{
    /// <summary>
    /// Can set Type as a member Property via SetType
    /// </summary>
    /// <typeparam name="TConcrete"></typeparam>
    public abstract class TypedReferenceBase<TConcrete> : ReferenceBase<TConcrete>, ITypedReference
        where TConcrete : ReferenceBase<TConcrete>
    {
        #region Type

        public override Type Type => type;
        private Type type;

        public void SetType(Type type)
        {
            if (this.type == type) return;
            if (type != default) throw new AlreadySetException();
            this.type = type;
        }

        #endregion
    }

    public abstract class TypedReferenceBase<TConcrete, TReferenced> : ReferenceBase<TConcrete>, ITypedReference
        where TConcrete : ReferenceBase<TConcrete>
    {
        public override Type Type => typeof(TReferenced);
    }

#if TOPORT
    public class AssetPathToVobReferenceResolver : IGetter<IResolvingReference, IReference>
    {
        public IReference Resolve<T>(T r) where T : IResolvingReference
        {
            new VobReference();
        }
    }
#endif

    // TODO: Document, maybe rename generic type names
    public abstract class ResolvingTypedReferenceBase<TConcrete, TValue> 
        : ReferenceBase<TConcrete>
        , ITypedReference
        , IGets
        where TConcrete : ReferenceBase<TConcrete>
    {
        public override Type Type => typeof(TValue);

        [Blocking(Alternative = nameof(ResolveReference))]
        public IReference ResolvedReference
        {
            get
            {
                if (resolvedReference == null)
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
                resolvedReference = (await ((TConcrete)(object)this).AmbientGet<TConcrete, IReference>().ConfigureAwait(false)).Value; // HARDCAST
            }
        }

        public override string Scheme => resolvedReference.Scheme;
        //public override string Host { get => resolvedReference.Host; set => throw new NotSupportedException(); }
        //public override string Port { get => resolvedReference.Port; set => throw new NotSupportedException(); }
        public override string Key { get => resolvedReference.Key; protected set => throw new NotSupportedException(); }
        public override string Path { get => resolvedReference.Path; protected set => throw new NotSupportedException(); }
    }
}
