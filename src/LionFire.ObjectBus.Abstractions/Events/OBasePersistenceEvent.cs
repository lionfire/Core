using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;

namespace LionFire.ObjectBus
{

    /// <summary>
    /// (REVIEW)
    /// </summary>
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
            get {
                //throw new NotImplementedException();
                return /*obj ?? */Handle?.Object<object>();
            }

            //set {
            //    throw new NotImplementedException();
            //    //obj = value;
            //}
        }
        //private object obj;

        #endregion

        #region Handle

        public IHandleBase Handle
        {
            get { return handle /* ?? Reference?.ToHandle()*/; }
            set { if (reference != null) { throw new AlreadyException("Cannot set if Reference is already set."); }  handle = value; }
        }
        private IHandleBase handle;

        #endregion
    }

}
