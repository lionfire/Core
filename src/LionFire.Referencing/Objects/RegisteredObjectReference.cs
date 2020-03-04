using System;

namespace LionFire.Referencing
{
    public class RegisteredObjectReference : IReference
    {
        #region Construction

        public RegisteredObjectReference() { }
        public RegisteredObjectReference(long id)
        {
            Id = id;
        }
        public RegisteredObjectReference(object obj) : this(ObjectReferenceRegistrar.Default.Register(obj))
        {
            Object = new WeakReference(obj);
        }

        #endregion


        #region Object

        [SetOnce]
        public object Object
        {
            get => obj;
            set
            {
                if (ReferenceEquals(obj, value)) return;
                if (obj != default) throw new AlreadySetException();
                obj = value;
            }
        }
        private object obj;

        #endregion

        public string Scheme => "object";

        public string Persister => null;

        public string Path => Id == 0 ? null : Id.ToString();

        public long Id { get; set; }

        public string Key => Path;

        public bool IsCompatibleWith(string obj) => throw new NotImplementedException();
    }
}
