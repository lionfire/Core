#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Activation;
using LionFire.Extensions.DefaultValues;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Persistence.Handles
{
    internal static class DllInternals // Move to LionFire.Data.Async.AsyncGetsWithEvents and 
    {
        internal static ValueChangedPropagation ValueChangedPropagation = new ValueChangedPropagation();
    }


    // REVIEW - why is there GetValue in WriteHandleBase? - should this inherit from Resolves<> and have NotSupported if truly write only?

    public abstract class WriteHandleBase<TReference, TValue> 
        //: DisposableKeyed<TReference>
        : Resolves<TReference, TValue>
        , IWriteHandleBase<TValue>
        , IWrapper<TValue>
        , IHandleInternal<TValue>
        , IDeletable
        , ISets<TValue>
        , IReferencable<TReference>
        , INotifyWrappedValueChanged
        , INotifyWrappedValueReplaced
        , IReferencableAsValueType<TValue>
        where TReference : IReference<TValue>
    {

        public Type Type => typeof(TValue);
        public IReference Reference => Key;
        TReference IReferencable<TReference>.Reference => Key;
        IReference<TValue> IReferencableAsValueType<TValue>.Reference => Key;

        #region Construction

        protected WriteHandleBase() { }
        protected WriteHandleBase(TReference reference) : base(reference)
        {
        }
        //protected WriteHandleBase(IReference reference) : base(reference)
        //{
        //}
        protected WriteHandleBase(TReference reference, TValue prestagedValue) : base(reference)
        {
            SetValueFromConstructor(prestagedValue);
        }

        /// <summary>
        /// Override this to disallow setting presresolved values in a constructor, or to take some other action
        /// </summary>
        /// <param name="initialValue"></param>
        protected virtual void SetValueFromConstructor(TValue initialValue)
        {
            ProtectedValue = initialValue;
            Flags |= PersistenceFlags.OutgoingUpsertPending;  // REVIEW: In the future, we may want to do something more specific here, like set something along the lines of PersistenceFlags.SetByUser
        }

        #endregion

        private readonly object persistenceLock = new object();

        #region Value

        public virtual TValue Value
        {
            [Blocking(Alternative = nameof(GetValue))]
            get => ProtectedValue ?? GetValue().Result.Value; // DUPLICATE of Resolves
            [PublicOnly]
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return; // Should use Equality instead of Compare?
                                                                                            //if (value == ProtectedValue) return;

                HandleUtils.OnUserChangedValue_Write(this, value);

            }
        }
        public event Action<INotifyWrappedValueReplaced, object, object> WrappedValueForFromTo;
        //public event Action<INotifyWrappedValueChanged> WrappedValueChanged;
        public event Action<INotifyWrappedValueChanged> WrappedValueChanged
        {
            add { Debug.WriteLine("WrappedValueChanged+=");  wrappedValueChanged += value; }
            remove { wrappedValueChanged -= value; }
        }
        private event Action<INotifyWrappedValueChanged> wrappedValueChanged;

        #region 


        /// <summary>
        /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
        /// </summary>
        public bool HasValue => !EqualityComparer<TValue>.Default.Equals(ProtectedValue, default);

        // DUPLICATE from Resolves
        protected TValue ProtectedValue
        {
            get => protectedValue;
            set
            {
                if (object.Equals(value, protectedValue)) return;
                //if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return; // Should use Equality instead of Compare?
                                                                                                                     //if (value == protectedValue) { return; }
                var oldValue = protectedValue;
                protectedValue = value;
                OnValueChanged(value, oldValue);
            }
        }
        /// <summary>
        /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
        /// </summary>
        protected TValue protectedValue;
        TValue IHandleInternal<TValue>.ProtectedValue { get => ProtectedValue; set => ProtectedValue = value; }

        /// <summary>
        /// Raised when ProtectedValue changes
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected virtual void OnValueChanged(TValue newValue, TValue oldValue)
        {
            //if (WrappedValueChanged != null || WrappedValueForFromTo != null) // FUTURE: Only do this if someone is attached to one or more of these events.  Would need to VCP.Attach upon initial attaching to one of these events.
            {
                DllInternals.ValueChangedPropagation.Detach(this, oldValue);
                DllInternals.ValueChangedPropagation.Attach(this, newValue, o =>
                {
                    wrappedValueChanged?.Invoke(this);
                    //this.OnUserChangedValue_ReadWrite(((WriteHandleBase<TReference, TValue>)o).protectedValue);
                    this.OnUserChangedValue_ReadWrite((TValue)o);
                });
                WrappedValueForFromTo?.Invoke(this, oldValue, newValue);
                wrappedValueChanged?.Invoke(this); // Assume that there was a change
            }
            this.OnUserChangedValue_ReadWrite(newValue);
        }

        #endregion

        #endregion

        #region GetValue

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        public async Task<ILazyGetResult<TValue>> GetValue()
        {
            var currentValue = ProtectedValue;
            if (currentValue != null) return new ResolveResultNoop<TValue>(ProtectedValue);

            var resolveResult = await Resolve().ConfigureAwait(false);
            return new LazyResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
        }

        #endregion

        #region Resolve

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        public async Task<IGetResult<TValue>> Resolve()
        {
            var resolveResult = await ResolveImpl();
            ProtectedValue = resolveResult.Value;
            return resolveResult;
        }

        #endregion

        #region Abstract

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        //protected abstract ITask<IGetResult<TValue>> ResolveImpl();

        #endregion

        public PersistenceFlags Flags { get; protected set; }
        PersistenceFlags IHandleInternal<TValue>.Flags { set => Flags = value; }

        public override void DiscardValue()
        {
            base.DiscardValue();
            Flags &= ~(
                PersistenceFlags.OutgoingCreatePending
                | PersistenceFlags.OutgoingDeletePending
                | PersistenceFlags.OutgoingUpdatePending
                | PersistenceFlags.OutgoingUpsertPending
                );
            throw new System.NotImplementedException();
        }

        async Task<ISuccessResult> ISets.Set() => await Put();
        public async Task<ISuccessResult> Set(TValue value, CancellationToken cancellationToken = default)
        {
            Value = value;
            return await Put().ConfigureAwait(false);
        }
        public virtual Task<ITransferResult> Put()
        {
            if (Flags.HasFlag(PersistenceFlags.OutgoingUpdatePending))
            {
                return UpdateImpl();
            }
            else
            if (Flags.HasFlag(PersistenceFlags.OutgoingUpsertPending))
            {
                return UpsertImpl();
            }
            else if (Flags.HasFlag(PersistenceFlags.OutgoingCreatePending))
            {
                return CreateImpl();
            }
            else if (Flags.HasFlag(PersistenceFlags.OutgoingDeletePending))
            {
                return DeleteImpl();
            }
            else
            {
                return Task.FromResult<ITransferResult>(NoopFailPersistenceResult<TValue>.Instance);
            }
        }

        #region Instantiation  - REVIEW for threadsafety and whether these belong here

        // THOUGHTS - I am not sure I want these methods here for instantiating values since it assumes how to create and even what type to create.
        // Could it be created by a service?  What about a static default?
        // The static type registry could either point to a static Func, or else a flag that says use DependencyContext.  ITypeActivator<T>  ITypeActivationTypeProvider<T>


        public ITypedReference TypedReference => Reference as ITypedReference; // REVIEW - does this belong here?  If this is non-null, it is queried when creating the Value on demand.  Maybe it belongs in the ReadWriteHandle.  // MOVE to ReadWriteHandle(?)

        //protected void DoPersistence(Action action)
        //{
        //    var oldValue = protectedValue;
        //    action();
        //    var newValue = Value;
        //    var newHasValue = HasValue;

        //    //if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) != 0)
        //    //{
        //    //    OnValue
        //    //}
        //    throw new NotImplementedException("Review logic of this, see if anything missing");

        //    //return newValue;
        //}

        //protected void TrySetProtectedValueIfDefault(TValue value)
        //{
        //    if (EqualityComparer<TValue>.Default.Equals(value))


        //    DoPersistence(() => Interlocked.CompareExchange<TValue>(ref protectedValue, value, default));
        //    //var oldValue = protectedValue;
        //    //var newValue = Interlocked.CompareExchange<T>(ref protectedValue, value, default);
        //    //OnValueChanged(newValue, oldValue);
        //    //return newValue;
        //}
        //protected T TrySetProtectedValueIfDefault<T>(T value) where T : class, T => Interlocked.CompareExchange<T>(ref protectedValue, value, default);

        // No persistence, just instantiating an ObjectType

        /// <summary>
        /// Returns null if ObjectType is object or interface and TypedReference?.Type is null
        /// TODO: If ObjectType is Interface, get create type from attribute on Interface type.
        /// </summary>
        public Type GetInstantiationType()
        {
            if (typeof(TValue) == typeof(object))
            {
                if (TypedReference?.Type == null)
                {
                    return null;
                }
                return (Type)typeof(TypeActivationConfiguration<>).MakeGenericType(TypedReference.Type).GetProperty(nameof(TypeActivationConfiguration<TValue>.ActivationType)).GetValue(null);
                //return TypedReference.Type;
            }
            else
            {
                return TypeActivationConfiguration<TValue>.ActivationType;
                //return typeof(T);
            }
        }

        protected TValue InstantiateDefault(bool applyDefaultValues = true)
        {
            TValue result = (TValue)Activator.CreateInstance(GetInstantiationType() ?? throw new ArgumentNullException("Reference.Type must be set when using non-generic Handle, or when the generic type is object."));

            if (applyDefaultValues) { DefaultValueUtils.ApplyDefaultValues(result); }

            return result;
        }

        public void InstantiateAndSet(bool applyDefaultValues = true) => Value = InstantiateDefault(applyDefaultValues);
        private void InstantiateAndSetWithoutEvents(bool applyDefaultValues = true) => Value = InstantiateDefault(applyDefaultValues);

        public void EnsureInstantiated() // REVIEW: What should be done here?
        {
            //RetrieveOrCreateDefault(); ??

            if (Value == null)
            {
                InstantiateAndSet();
            }
        }
        //private void EnsureInstantiatedWithoutEvents() // REVIEW: What should be done here?
        //{
        //    if (_value == null)
        //    {
        //        InstantiateAndSetWithoutEvents();
        //    }
        //}

        #endregion

        public async Task<bool?> CommitDelete() => (await DeleteImpl().ConfigureAwait(false)).IsFound(); // REVIEW - superfluous? or does it belong in an interface?

        public virtual void MarkDeleted() => this.OnUserChangedValue_Write(default);

        #region Abstract Methods

        protected abstract Task<ITransferResult> UpsertImpl();

        /// <summary>
        /// Default implementation is to defer to UpsertImpl, which should set the value to the default value.  Override this for stores that have a dedicated delete API.
        /// </summary>
        protected virtual Task<ITransferResult> DeleteImpl() => UpsertImpl();

        /// <summary>
        /// Default implementation is to defer to UpsertImpl.
        /// </summary>
        protected virtual Task<ITransferResult> CreateImpl() => UpsertImpl();

        /// <summary>
        /// Default implementation is to defer to UpsertImpl.
        /// </summary>
        protected virtual Task<ITransferResult> UpdateImpl() => UpsertImpl();

        #endregion

        #region Primitive Interface Implementations

        async Task<bool?> IDeletable.Delete()
        {
            MarkDeleted();
            var putResult = await Put();
            return putResult.Flags.IsFound();
        }

        #endregion
    }
}
