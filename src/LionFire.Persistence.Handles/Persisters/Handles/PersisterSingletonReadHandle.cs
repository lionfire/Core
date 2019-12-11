#if UNUSED
using LionFire.Dependencies;
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    public class PersisterSingletonReadHandle<TReference, TValue, TPersister> : PersisterReadHandle<TReference, TValue>, IPersisterHandle<TReference, TPersister>
        where TReference : IReference
        where TPersister : class, IPersister<TReference>
    {

        public PersisterSingletonReadHandle(TReference reference) : base(reference)
        {
        }

        public override IPersister<TReference> Persister
        {
            get => persister;
            protected set => persister = (TPersister)value;
        }

        #region persister

        [SetOnce]
        public TPersister persister
        {
            get
            {
                if (_persister == null)
                {
                    _persister = DependencyLocator.Get<TPersister>();
                }
                return _persister;
            }
            protected set
            {
                if (_persister == value) return;
                if (_persister != default) throw new AlreadySetException();
                _persister = value;
            }
        }
        private TPersister _persister;

        #endregion

        TPersister IPersisterHandle<TReference, TPersister>.Persister => persister;
    }

}
#endif