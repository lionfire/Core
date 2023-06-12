//using System;
//using System.Threading.Tasks;
//using LionFire.ObjectBus;
//using LionFire.Ontology;
//using LionFire.Persistence;
//using LionFire.Persistence.Handles;
//using LionFire.Referencing;
//using LionFire.Data.Async.Gets;

//namespace LionFire.ObjectBus.Handles
//{
//    public class OBaseHandle<T> : PersisterReadWriteHandle<IReference, T>, IHas<IOBase>
//    {
//        public IOBase OBase { get;  }
//        IOBase IHas<IOBase>.Object => OBase;


//        //#region Persister

//        //[SetOnce]
//        //public override IPersister<IReference> Persister
//        //{
//        //    get => persister;
//        //    protected set
//        //    {
//        //        if (persister == value) return;
//        //        if (persister != default) throw new AlreadySetException();
//        //        persister = value;
//        //    }
//        //}
//        //private IPersister<IReference> persister;

//        //#endregion

//        public OBaseHandle(IReference reference, IOBase obase, T handleObject = default) : base(obase.GetPersister<IReference>(), reference)
//        {
//            this.OBase = obase ?? throw new ArgumentNullException(nameof(obase));
//            Value = handleObject;
//        }

//        // Some code duplication with OBaseReadHandle
//        protected override async Task<IGetResult<T>> ResolveImpl() 
//            => await OBase.Get<T>(this.Reference).ConfigureAwait(false);

//        //protected async Task<IPersistenceResult> DeleteObject()
//        //    => await OBase.TryDelete<T>(this.Reference).ConfigureAwait(false);

//        protected async Task<IPersistenceResult> DeleteObject() // TODO: Override something? 
//            => await OBase.TryDelete<T>(this.Reference).ConfigureAwait(false);

//        protected override async Task<IPersistenceResult> UpsertImpl()
//        {
//            // TODO: propagate persistenceContext?  Or remove it and rely on ambient AsyncLocal?
//            var result = await OBase.Set<T>(this.Reference, ProtectedValue).ConfigureAwait(false);
//            if (!result.IsSuccess()) throw new PersistenceException(result, "Failed to persist.  See Result for more information.");
//            return result;
//        }
//    }
//}
