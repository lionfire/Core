using LionFire.Referencing.Resolution;
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing
{

    /// <summary>
    /// Uses ReferenceResolver (with fallback to ReferencingConfig.DefaultReferenceResolver()) to resolve Object.
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public class RDynamic<ObjectType> : HRetrieveInfoBase<ObjectType>, IHasObjectRetrievalInfo<ObjectType>
        where ObjectType : class
    {
        #region Construction

        public RDynamic() { }
        public RDynamic(ObjectType obj) : base(obj) { }
        public RDynamic(IReference reference, ObjectType obj) : base(obj)
        {
            this.reference = reference;
        }
        public RDynamic(IReference reference) : base()
        {
            this.reference = reference;
        }

        #endregion

        public override string Key { get => Reference?.Key; set => throw new NotImplementedException("TODO: Create an IReference from Key, perhaps via a UrlReference class "); }

        #region Reference

        public override IReference Reference
        {
            get { return reference; }
            set
            {
                if (reference == value)
                {
                    return;
                }

                if (reference != default(IReference))
                {
                    throw new AlreadySetException();
                }

                reference = value;
            }
        }
        private IReference reference;

        #endregion

        public virtual IHandleResolver EffectiveReferenceResolver
        {
            get
            {
                return this.ReferenceResolver ?? (this.Reference as IResolvingReference)?.HandleResolver ?? ReferencingConfig.DefaultReferenceResolver();
            }
        }

        public virtual IHandleResolver ReferenceResolver
        {
            get; set;
        }

        public override async Task<bool> TryResolveObject()
        {
            return (await TryResolveObjectWithInfo().ConfigureAwait(false)).IsSuccess;
        }

        public override async Task<ResolveHandleResult<ObjectType>> TryResolveObjectWithInfo()
        {
            var result = await EffectiveReferenceResolver.Resolve(this).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.Object = result.Result;
                this.ResolveHandleResult = result;
                this.IsResolved = true;
            }
            else
            {
                ResolveHandleResult = null;
                IsResolved = false;
                //if (forgetOnFail)
                //{
                //    this.ForgetObject();
                //}
            }
            return result;
        }
    }
}
