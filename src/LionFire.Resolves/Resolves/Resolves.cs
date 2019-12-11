using MorseCode.ITask;
using System.Collections.Generic;
using System.Threading;

namespace LionFire.Resolves
{
    // REVIEW - Is this still needed with covariant ITask?
    //public abstract class Resolves<TKey, TValue, TValueReturned> : DisposableKeyed<TKey>
    //    where TValueReturned : TValue


    /// <summary>
    /// Only requires one method to be implemented: ResolveImpl.
    /// </summary>
    public abstract class Resolves<TKey, TValue> : DisposableKeyed<TKey>
         where TKey : class
    {
        #region Construction

        protected Resolves() { }
        protected Resolves(TKey input) : base(input) { }

        ///// <summary> // Is this needed/helpful?
        ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="initialValue"></param>
        //protected Resolves(TKey input, TValue initialValue) : base(input)
        //{
        //    if (initialValue != default)
        //    {
        //        ProtectedValue = initialValue;
        //    }
        //}

        #endregion

        #region Value

        /// <summary>
        /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
        /// </summary>
        public bool HasValue => !EqualityComparer<TValue>.Default.Equals(ProtectedValue, default);

        public TValue Value
        {
            [Blocking(Alternative = nameof(GetValue))]
            get => ProtectedValue ?? GetValue().Result.Value;
        }

        protected TValue ProtectedValue
        {
            get => protectedValue;
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return;
                var oldValue = protectedValue;
                protectedValue = value;
                OnValueChanged(value, oldValue);
            }
        }
        /// <summary>
        /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
        /// </summary>
        protected TValue protectedValue;

        /// <summary>
        /// Raised when ProtectedValue changes
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        protected virtual void OnValueChanged(TValue newValue, TValue oldValue) { }

        #endregion

        #region GetValue

        public async ITask<ILazyResolveResult<TValue>> GetValue()
        {
            var currentValue = ProtectedValue;
            if (currentValue != null) return new ResolveResultNoop<TValue>(ProtectedValue);

            var resolveResult = await Resolve();
            return new ResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
        }

        #endregion

        #region QueryValue

        public ILazyResolveResult<TValue> QueryValue()
        {
            var currentValue = ProtectedValue;
            return currentValue != null ? new ResolveResultNoop<TValue>(ProtectedValue) : (ILazyResolveResult<TValue>)ResolveResultNotResolved<TValue>.Instance;
        }

        #endregion

        #region Discard

        public virtual void DiscardValue() => ProtectedValue = default;

        #endregion

        #region Resolve

        public async ITask<IResolveResult<TValue>> Resolve()
        {
            var resolveResult = await ResolveImpl();
            ProtectedValue = resolveResult.Value;
            return resolveResult;
        }

        #endregion

        #region Abstract

        protected abstract ITask<IResolveResult<TValue>> ResolveImpl();

        #endregion

        #region OLD

        //public async ITask<ILazyResolveResult<TValueReturned>> GetValue2()
        //{
        //    var currentValue = ProtectedValue;
        //    if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

        //    var resolveResult = await Resolve();
        //    return new LazyResolveResult<TValueReturned>(resolveResult.HasValue, (TValueReturned)(object)resolveResult.Value);
        //}

        #endregion
    }

}

