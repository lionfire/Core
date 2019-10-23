using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public abstract class ResolvesBase<TKey, TValue> : ResolvesBase<TKey,TValue, TValue>, ILazilyResolves<TValue>
    {        
    }

    public abstract class ResolvesBase<TKey, TValue, TValueReturned> : ResolvesInputBase<TKey>
    {
        #region Construction

        protected ResolvesBase() { }
        protected ResolvesBase(TKey input) : base(input) { }

        #endregion

        public TValue Value => ProtectedValue ?? (TValue)(object)GetValue().Result.Value; // HARDCAST

        public async Task<ILazyResolveResult<TValueReturned>> GetValue()
        {
            var currentValue = ProtectedValue;
            if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

            var resolveResult = await Resolve();
            return new LazyResolveResult<TValueReturned>(resolveResult.HasValue, (TValueReturned)(object)resolveResult.Value);
        }

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

        public bool HasValue => ProtectedValue != default;

        public void DiscardValue() => ProtectedValue = default;

        public async Task<IResolveResult<TValue>> Resolve()
        {
            var resolveResult = await ResolveImpl();
            ProtectedValue = resolveResult.Value;
            return resolveResult;
        }

        public abstract Task<IResolveResult<TValue>> ResolveImpl();
    }

}

