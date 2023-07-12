using LionFire.Persistence;
using LionFire.Persistence.Implementation;
using LionFire.Referencing;
using LionFire.Data.Gets;
using LionFire.Threading;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;
using LionFire.Data.Sets;

namespace LionFire.Persistence.Handles;

public abstract class ReadWriteHandleBase<TReference, TValue>
    : WriteHandleBase<TReference, TValue>
    , IReadWriteHandleBase<TValue>
    , IPersistsInternal
    , IHandleInternal<TValue>    
    where TReference : IReference<TValue>
{
    #region Construction

    public ReadWriteHandleBase() { }
    public ReadWriteHandleBase(TReference reference) : base(reference) { }
    //public ReadWriteHandleBase(IReference reference) : base(reference) { }
    public ReadWriteHandleBase(TReference reference, TValue preresolvedValue) : base(reference, preresolvedValue) { }

    #endregion

    #region Value

    public override TValue Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => HasValue ? ReadCacheValue : GetIfNeeded().Result.Value;
        [PublicOnly]
        set
        {
            if (object.Equals(value, protectedValue)) return;
            //if (System.Collections.Generic.Comparer<TValue>.Default.Compare(readCacheValue, value) == 0) return; // Should use Equality instead of Compare?
            //if (value == ReadCacheValue) return;
            this.StageValue_ReadWrite(value);
        }
    }
    //TValue IHandleInternal<TValue>.ProtectedValue { get => ProtectedValue; set => ProtectedValue = value; }
    PersistenceFlags IPersistsInternal.Flags { get => Flags; set => Flags = value; }

    #endregion

    /// <summary>
    /// Returns GetValue().Value if HasValue is true, otherwise uses InstantiateDefault() to populate Value and returns that.
    /// </summary>
    /// <returns>A guaranteed Value, that may have been preexisting, lazily loaded, or just instantiated.</returns>
    [ThreadSafe]
    public async ITask<IGetResult<TValue>> GetOrInstantiateValue()
    {
        var getResult = await GetIfNeeded().ConfigureAwait(false);
        if (getResult.HasValue) return getResult;

        //TrySetProtectedValueIfDefault(InstantiateDefault());
        //ReadCacheValue = InstantiateDefault();
        var newValue = InstantiateDefault();
        this.StageValue_Write_Old(newValue);

        return RetrieveResult<TValue>.NotFoundButInstantiated(newValue);

        // Consider .NET's LazyInitializer
        //lock (objectLock)
        //{
        //    if (!HasValue)
        //    {
        //        ReadCacheValue = InstantiateDefault();
        //    }
        //    return ReadCacheValue;
        //}
    }

    async Task<bool?> IDeletable.Delete()
    {
        MarkDeleted();
        var putResult = await Put();
        return putResult.IsFound();
    }

    void IDeletable.MarkDeleted() => this.StageValue_ReadWrite(default);


    ITask<IGetResult<TValue>> IGets<TValue>.Get(CancellationToken cancellationToken = default) => Get(cancellationToken);
}

#if OLD
public abstract class ReadWriteHandleBase<TValue> : Resolves<IReference, TValue>, IReadHandleBase<TValue>, IReadWriteHandleBase<TValue>
     //Resolves<IReference, TValue>, RH<TValue>, IReadHandleInvariant<TValue>
    //, ICommitableImpl, IDeletableImpl
{
#region Reference

    protected virtual bool IsAllowedReferenceType(Type type) => true;

    [SetOnce]
    public IReference Reference
    {
        get => reference;
        protected set
        {
            if (reference == value)
            {
                return;
            }

            if (reference != default(IReference))
            {
                throw new AlreadySetException();
            }

            // OLD: art != null && value != null && !art.Where(type => type.IsAssignableFrom(value.GetType())).Any()
            if (!IsAllowedReferenceType(value.GetType()))
            {
                throw new ArgumentException("This type does not support IReference types of that type.  See protected IsAllowedReferenceType implementation for allowed types.");
            }

            reference = value;
        }
    }
    protected IReference reference;

#endregion

#region Persistence State

    public PersistenceFlags Flags
    {
        get => handleState;
        set
        {
            if (handleState == value) { return; }

            var oldValue = handleState;
            handleState = value;

            OnStateChanged(value, oldValue);
        }
    }

    TValue IWrapper<TValue>.Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    TValue IWriteWrapper<TValue>.Value { set => throw new NotImplementedException(); }

    private PersistenceFlags handleState;

    protected virtual void OnStateChanged(PersistenceFlags newValue, PersistenceFlags oldValue) { }
    
#endregion

#region Construction

    protected ReadWriteHandleBase() { }

    protected ReadWriteHandleBase(IReference input) : base(input) { }

    ///// <summary>
    ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
    ///// </summary>
    ///// <param name="input"></param>
    ///// <param name="initialValue"></param>
    //protected ReadWriteHandleBase(IReference input, TValue initialValue) : base(input, initialValue)
    //{
    //}

#endregion
}
#endif
