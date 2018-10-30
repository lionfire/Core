using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public struct OBasePersistenceEvent
    {
        //public IReference Reference { get; set; }

        public PersistenceEventKind Kind { get; set; }
        public PersistenceEventSourceKind SourceKind { get; set; }

        #region Reference

        public IReference Reference
        {
            get { return reference ?? Handle?.Reference; }
            set { if (handle!=null) { throw new AlreadyException("Cannot set if Handle is already set."); } reference = value; }
        }
        private IReference reference;

        #endregion

        #region Object

        public object Object
        {
            get { return obj ?? Handle?.Object; }
            set { obj = value; }
        }
        private object obj;

        #endregion

        #region Handle

        public IHandle Handle
        {
            get { return handle /* ?? Reference?.ToHandle()*/; }
            set { if (reference != null) { throw new AlreadyException("Cannot set if Reference is already set."); }  handle = value; }
        }
        private IHandle handle;

        #endregion
    }

}
