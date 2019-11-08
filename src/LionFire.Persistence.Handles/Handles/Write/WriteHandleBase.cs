using System;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Activation;
using LionFire.Extensions.DefaultValues;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
using LionFire.Threading;

namespace LionFire.Persistence.Handles
{
    public abstract class WriteHandleBase<TReference, TValue> : DisposableKeyed<TReference>, IWriteHandleBase<TValue>, IWrapper<TValue>, INotifyPersists<TValue>, IHandleInternal<TValue>
        where TReference : class, IReference
        where TValue : class
    {
        public IReference Reference => Key;

        private readonly object persistenceLock = new object();
        object IHandleInternal<TValue>.persistenceLock => persistenceLock;

        #region Value

        public virtual TValue Value
        {
            [Blocking(Alternative = nameof(GetValue))]
            get => ProtectedValue ?? GetValue().Result.Value; // DUPLICATE of Resolves
            [PublicOnly]
            set
            {
                if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return; // Should use Equality instead of Compare?
                //if (value == ProtectedValue) return;
                this.MutatePersistenceState(() => HandleUtils.OnUserChangedValue_Write(this, value));
            }
        }

        #region 

        /// <summary>
        /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
        /// </summary>
        public bool HasValue => ProtectedValue != default;

        // DUPLICATE from Resolves
        protected TValue ProtectedValue
        {
            get => protectedValue;
            set
            {
                if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return; // Should use Equality instead of Compare?
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
        protected virtual void OnValueChanged(TValue newValue, TValue oldValue) { }

        #endregion

        public event Action<PersistenceEvent<TValue>> PersistenceStateChanged;
        void IHandleInternal<TValue>.RaisePersistenceStateChanged(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);

        #endregion

        #region GetValue

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        public async Task<ILazyResolveResult<TValue>> GetValue()
        {
            var currentValue = ProtectedValue;
            if (currentValue != null) return new ResolveResultNoop<TValue>(ProtectedValue);

            var resolveResult = await Resolve();
            return new ResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
        }

        #endregion

        #region Resolve

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        public async Task<IResolveResult<TValue>> Resolve()
        {
            var resolveResult = await ResolveImpl();
            ProtectedValue = resolveResult.Value;
            return resolveResult;
        }

        #endregion

        #region Abstract

        // DUPLICATE of Resolves, almost.  Returns Task instead of ITask.
        public abstract Task<IResolveResult<TValue>> ResolveImpl();

        #endregion

        public PersistenceFlags Flags { get; protected set; }
        PersistenceFlags IHandleInternal<TValue>.Flags { get => Flags; set => Flags = value; }

        //protected void DoPersistence(Action action)
        //{
        //    var oldValue = protectedValue;
        //    action();
        //    var newValue = Value;
        //    var newHasValue = HasValue;

        //    if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) != 0)
        //    {
        //        OnValue
        //    }

        //    return newValue;
        //}

        public void DiscardValue()
        {
            Flags &= ~(
                PersistenceFlags.OutgoingCreatePending
                | PersistenceFlags.OutgoingDeletePending
                | PersistenceFlags.OutgoingUpdatePending
                | PersistenceFlags.OutgoingUpsertPending
                );
            throw new System.NotImplementedException();
        }
        public Task<IPutResult> Put()
        {
            throw new System.NotImplementedException();
        }


        #region Instantiation  - REVIEW for threadsafety and whether these belong here

        // THOUGHTS - I am not sure I want these methods here for instantiating values since it assumes how to create and even what type to create.
        // Could it be created by a service?  What about a static default?
        // The static type registry could either point to a static Func, or else a flag that says use DependencyContext.  ITypeActivator<T>  ITypeActivationTypeProvider<T>


        public ITypedReference TypedReference => Reference as ITypedReference; // REVIEW - does this belong here?  If this is non-null, it is queried when creating the Value on demand.  Maybe it belongs in the ReadWriteHandle.  // MOVE to ReadWriteHandle(?)



        protected void TrySetProtectedValueIfDefault(TValue value)
        {
            DoPersistence(() => Interlocked.CompareExchange<TValue>(ref protectedValue, value, default));
            //var oldValue = protectedValue;
            //var newValue = Interlocked.CompareExchange<T>(ref protectedValue, value, default);
            //OnValueChanged(newValue, oldValue);
            //return newValue;
        }
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

        public Task<bool> Delete() => throw new NotImplementedException();
        public void MarkDeleted() => throw new NotImplementedException();
        public Task<IPutResult> Put(TValue value) => throw new NotImplementedException();
    }
}
