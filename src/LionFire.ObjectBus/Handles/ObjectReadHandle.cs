using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Handles
{
    /// <summary>
    /// Simple wrapper around an object to support the IReadHandle interface.
    /// </summary>
    /// <remarks>MOVE</remarks>
    /// <typeparam name="T"></typeparam>
    public class ObjectHandle<T> : IReadHandle<T>
        where T : class
    {
        #region Construction

        public ObjectHandle() { }
        public ObjectHandle(T obj) { this.Object = obj; }

        #endregion

        public bool HasObject {
            get {
                return Object != null;
            }
        }

        public T Object {
            get; set;
        }

        public T ObjectField {
            get { return Object; }
            set { Object = value; }
        }

        public IReference Reference {
            get { return null; }
        }

        object IReadHandle.Object {
            get {
                return Object;
            }
        }
    }
}
