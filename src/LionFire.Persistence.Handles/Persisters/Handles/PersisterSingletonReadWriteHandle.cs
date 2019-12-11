//using LionFire.Dependencies;
//using LionFire.Referencing;
//using System;

//namespace LionFire.Persistence.Handles
//{

//    public class PersisterSingletonReadWriteHandle<TReference, TValue, TPersister> : PersisterReadWriteHandle<TReference, TValue>, IPersisterHandle<TReference, TPersister>
//        where TReference : IReference
//        where TPersister : class, IPersister<TReference>
//    {

//        public PersisterSingletonReadWriteHandle(TReference reference) : base(reference)
//        {
//        }

//        public override IPersister<TReference> Persister
//        {
//            get => persister;
//            protected set => persister = (TPersister) value;
//        }

//        #region Persister

//        [SetOnce]
//        public TPersister persister
//        {
//            get
//            {
//                if (_persister == null)
//                {
//                    _persister = DependencyLocator.Get<TPersister>();
//                }
//                return _persister;
//            }
//            protected set
//            {
//                if (_persister == value) return;
//                if (_persister != default) throw new AlreadySetException();
//                _persister = value;
//            }
//        }
//        private TPersister _persister;

//        #endregion

//        TPersister IPersisterHandle<TReference, TPersister>.Persister => persister;
//    }

//}
