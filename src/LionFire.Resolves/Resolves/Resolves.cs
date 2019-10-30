using MorseCode.ITask;

namespace LionFire.Resolves
{
    // REVIEW - Is this still needed with covariant ITask?
    //public abstract class Resolves<TKey, TValue, TValueReturned> : ResolvesInputBase<TKey>
    //    where TValueReturned : TValue


    /// <summary>
    /// Only requires one method to be implemented: ResolveImpl.
    /// </summary>
    public abstract class Resolves<TKey, TValue> : ResolvesInputBase<TKey>
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
        /// For nullable values, use TValue of DefaultableValue&lt;TValue&gt;
        /// </summary>
        public bool HasValue => ProtectedValue != default;

        [Blocking(Alternative = nameof(GetValue))]
        public TValue Value => ProtectedValue ?? GetValue().Result.Value;

        protected TValue ProtectedValue
        {
            get => protectedValue;
            set
            {
                if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return;
                var oldValue = protectedValue;
                protectedValue = value;
                OnValueChanged(value, oldValue);
            }
        }
        private TValue protectedValue;

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
            if (currentValue != null) return new LazyResolveResultNoop<TValue>(ProtectedValue);

            var resolveResult = await Resolve();
            return new LazyResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
        }

        //public async ITask<ILazyResolveResult<TValueReturned>> GetValue2()
        //{
        //    var currentValue = ProtectedValue;
        //    if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

        //    var resolveResult = await Resolve();
        //    return new LazyResolveResult<TValueReturned>(resolveResult.HasValue, (TValueReturned)(object)resolveResult.Value);
        //}

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

        public abstract ITask<IResolveResult<TValue>> ResolveImpl();

        #endregion
    }
}

