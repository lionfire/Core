#if GenericVob
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{

    public partial class Vos
    {
        public Vob<T> AsVobType<T>() // TODO REVIEW FUTURE Use ((IReadOnlyMultiTyped)this).AsType{T}() instead
            where T : class, new()
        {
            throw new NotImplementedException();
        }
    }

    // Is this a good idea?  Maybe use non-generic class Vob: Vob.AsType{T}().  Use VobHandle<> instead?
    public class Vob<T> :
         H<T> // REVIEW: H<> vs IHasHandle<>
        where T : class, new()
    {
        private Vob vob;
        private Vos vos;

#region Construction

        public Vob(Vob mainVob) { this.vob = mainVob; }

        //public Vob(Vos vos, Vob vob, H<T> objectHandle)
        //{
        //    // TODO: Complete member initialization
        //    this.vos = vos;
        //    this.vob = vob;
        //    this.objectHandle = objectHandle;

        //}

        //REVIEW TODO: Provide this ctor?  also in base?
        //public Vob(VobReference reference) : base(vos, parent, name) { this.Reference = reference; }

#endregion

        //Handle<T> handle;

#region Handle pass-through

        public void AssignFrom(T other)
        {
            vob.AssignFrom(other);
        }

        public T Object
        {
            get
            {
                return vob.AsType<T>();
            }
            set
            {
                vob.Object = value;
            }
        }

        object H.Object
        {
            get
            {
                return vob.Object;
            }
            set
            {
                vob.Object = value;
            }
        }

        public bool HasObject
        {
            get { return vob.HasObject; }
        }

        public event Action<IChangeableReferenceable> ReferenceChanged
        {
            add { vob.ReferenceChanged += value; }
            remove { vob.ReferenceChanged -= value; }
        }

        public void AssignFrom(object other)
        {
            vob.AssignFrom(other);
        }

        public bool RetrieveOnDemand
        {
            get
            {
                return vob.RetrieveOnDemand;
            }
            set
            {
                vob.RetrieveOnDemand = value;
            }
        }

        public void Get()
        {
            vob.Get();
        }

        public object TryRetrieve()
        {
            return vob.TryRetrieve();
        }

        public void EnsureRetrieved()
        {
            vob.EnsureRetrieved();
        }

        public bool TryEnsureRetrieved()
        {
            return vob.TryEnsureRetrieved();
        }

        public void RetrieveOrCreate()
        {
            vob.RetrieveOrCreate();
        }

        public void RetrieveOrCreateDefault(object defaultValue)
        {
            vob.RetrieveOrCreateDefault(defaultValue);
        }

        public void ConstructDefault()
        {
            vob.ConstructDefault();
        }

        public void Delete()
        {
            vob.Delete();
        }

        public void Move(IReference newReference)
        {
            vob.Move(newReference);
        }

        public void Copy(IReference newReference)
        {
            vob.Copy(newReference);
        }

        public bool AutoSave
        {
            get
            {
                return vob.AutoSave;
            }
            set
            {
                vob.AutoSave = value;
            }
        }

        public void Save()
        {
            vob.Save();
        }

        public void RetrieveOrCreateDefault(T defaultValue)
        {
            vob.RetrieveOrCreateDefault(defaultValue);
        }

        public IEnumerable<H> GetChildren()
        {
            return vob.GetChildren();
        }

        public IEnumerable<H<ChildType>> GetChildrenOfType<ChildType>() where ChildType : class, new()
        {
            return vob.GetChildrenOfType<ChildType>();
        }

        public IEnumerable<string> GetChildrenNamesOfType<ChildType>() where ChildType : class, new()
        {
            return vob.GetChildrenNamesOfType<ChildType>();
        }

#endregion

        public H this[string subpath]
        {
            get { return vob[subpath]; }
        }

        public H this[IEnumerator<string> subpathChunks]
        {
            get { return vob[subpathChunks]; }
        }

        public H this[IEnumerable<string> subpathChunks]
        {
            get { return vob[subpathChunks]; }
        }

        public H this[int index, string[] subpathChunks]
        {
            get { return vob[index, subpathChunks]; }
        }

        public H this[params string[] subpathChunks]
        {
            get { return vob[subpathChunks]; }
        }

        public IReference Reference
        {
            get
            {
                return vob.Reference;
            }
            set
            {
                vob.Reference = value;
            }
        }


        bool IHandlePersistence.TryRetrieve()
        {
            return ((IHandlePersistence)vob).TryRetrieve();
        }

        public bool TryDelete()
        {
            return vob.TryDelete();
        }

        public IEnumerable<string> GetChildrenNames()
        {
            return vob.GetChildrenNames();
        }

        public H Handle
        {
            get
            {
                return vob.Handle;
            }
            set
            {
                vob.Handle = value;
            }
        }

        public object this[Type type]
        {
            get { return vob[type]; }
        }

        public T AsType<T>() where T : class
        {
            return vob.AsType<T>();
        }

        public T[] OfType<T>() where T : class
        {
            return vob.OfType<T>();
        }

        public IEnumerable<object> SubTypes
        {
            get { return vob.SubTypes; }
        }
    }
}
#endif
