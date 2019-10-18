using LionFire.Persistence;
using LionFire.Results;
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Resolvables
{

    public class ResolvableBase<TInput, TOutput>
    {
        public static implicit operator ResolvableBase<TInput, TOutput>(TInput input) => new Resolvable<TInput, TOutput>(input);

        protected ResolvableBase() { }
        protected ResolvableBase(TInput input) { this.Input = input; }

        #region Input

        [SetOnce]
        public TInput Input
        {
            get => input;
            set
            {
                if (ReferenceEquals(input, value)) return;
                if (input != default) throw new AlreadySetException();
                input = value;
            }
        }
        protected TInput input;

        #endregion
    }

    /// <summary>
    /// Typical implementation of Resolvable, implementing ILazyRetrievable
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    // TODO: Derive from RBase instead?  Or the other way around: migrate toward RBase<TValue> is a IResolvable<IReference, TValue>?
    public class Resolvable<TKey, TValue> : ResolvableBase<TKey, TValue>, IResolvesEx, ILazilyRetrievable<TValue>, ILazilyRetrievableInvariant<TValue>
    {
        public static implicit operator Resolvable<TKey, TValue>(TKey input) => new Resolvable<TKey, TValue>(input);
        
        public Resolvable() { }
        public Resolvable(TKey input) : base(input) { }


        #region State

        public PersistenceState State
        {
            get => state;
            set
            {
                if (state == value) return;
                state = value;
                // TODO: Implement IHasPersistenceStateEvents
            }
        }
        private PersistenceState state;

        #endregion

        #region Output

        public TValue Object
        {
            get => Get().Result.Object;
            protected set => @object = value;
        }
        private TValue @object;

        #region RetrievedNull

        public bool RetrievedNull
        {
            get => retrievedNull;
            protected set => retrievedNull = value;
        }
        private bool retrievedNull;

        #endregion

        #endregion

        #region Input

        public bool HasValue => !ReferenceEquals(@object, default) || RetrievedNull;

        public async Task<(bool HasObject, TValue Object)> Get()
        {
            if (state.HasFlag(PersistenceState.UpToDate))
            {
                return (true, @object);
            }

            await ResolveAsync();

            return (HasValue, @object);
        }
        async Task<(bool HasObject, object Object)> ILazilyRetrievable<TValue>.Get()
        {
            var result = await this.Get();
            return (result.HasObject, result.Object);
        }

        public void DiscardObject()
        {
            retrievedNull = false;
            @object = default;
        }

        public Task<IResolveResult> ResolveAsync()
        {
            this.Object = Input.Resolve<TKey, TValue>();
            return SuccessResult.Success.ToResult<IResolveResult>();
        }

        public Task<bool> Exists(bool forceCheck = false) => throw new System.NotImplementedException();

        #endregion
    }
}
