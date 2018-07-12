using LionFire.Referencing.Resolution;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public abstract class HDynamicBase<ObjectType> : HRetrieveInfoBase<ObjectType>, IHasObjectRetrievalInfo<ObjectType>
        where ObjectType : class
    {
        public virtual IHandleResolver EffectiveReferenceResolver
        {
            get
            {
                return this.ReferenceResolver ?? this.Reference?.HandleResolver ?? ReferencingConfig.DefaultReferenceResolver();
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
