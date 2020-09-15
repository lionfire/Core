using System;

namespace LionFire.Referencing
{
    public class RegisteredObjectWeakReference : IReference
    {
        #region Construction

        public RegisteredObjectWeakReference() { }
        public RegisteredObjectWeakReference(long id)
        {
            Id = id;
        }
        public RegisteredObjectWeakReference(object obj) : this(ObjectReferenceRegistrar.Default.Register(obj))
        {
            Object = new WeakReference(obj);
        }

        #endregion

        #region Object

        [SetOnce]
        public object Object
        {
            get => weakReference.Target;
            set
            {
                if (weakReference.Target == value) return;
                if (weakReference != default) throw new AlreadySetException();
                weakReference = new WeakReference(value);
            }
        }
        private WeakReference weakReference;

        #endregion

        public string Scheme => "object";
        public const string SchemePrefix = "object:";

        public string Persister => null; // TODO: Allow other persisters and make sure to include it in the Key

        public string Path => Id == 0 ? null : Id.ToString();

        public long Id { get; set; }

        public string Key => Path;
        public string Url => SchemePrefix + Path;

        public bool IsCompatibleWith(string obj) => throw new NotImplementedException();
    }
}
