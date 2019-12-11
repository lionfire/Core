//using LionFire.Referencing;

//namespace LionFire.Persistence.Handles
//{
//    public class PersisterInstanceReadWriteHandle<TReference, TValue> 
//        : PersisterReadWriteHandle<TReference, TValue>
//        where TReference : IReference
//    {
//        #region Persister

//        [SetOnce]
//        public override IPersister<TReference> Persister
//        {
//            get => persister;
//            protected set
//            {
//                if (persister == value) return;
//                if (persister != default) throw new AlreadySetException();
//                persister = value;
//            }
//        }
//        private IPersister<TReference> persister;

//        #endregion

//        public PersisterInstanceReadWriteHandle(IPersister<TReference> persister, TReference reference) : base(reference) => Persister = persister;

//    }
//}
