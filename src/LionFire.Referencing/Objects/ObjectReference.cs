using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    /// <summary>
    /// Represents a .NET reference to an object.  
    /// Key comes from IKeyed.Key if it exists.  Key/path are likely useless here -- use RegisteredObjectReference or RegisteredObjectWeakReference if you need a key.
    /// </summary>
    public class ObjectReference : IReference, ITypedReference
    {
        #region Construction

        public ObjectReference() { }
        public ObjectReference(object obj)
        {
            Object = obj;
        }

        #endregion

        public string Scheme => "object";

        public string Persister => null;

        public string Path => Key;

        public string Key => (obj as IKeyed).Key;

        #region Object

        [SetOnce]
        public object Object
        {
            get => obj;
            set
            {
                if (obj == value) return;
                if (obj != default) throw new AlreadySetException();
                obj = value;
            }
        }
        private object obj;

        #endregion

        public Type Type => obj?.GetType();

        public bool IsCompatibleWith(string obj) => false;
    }
}
