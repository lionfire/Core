//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using LionFire.Types;

//namespace LionFire.ObjectBus
//{
//    //public interface IONode
//    //{
//    //}


//    public class DynamicTreeNode : IReadonlyMultiTyped, IDirectory,
//        IReferenceable, IHasHandle
//    {

//        #region Construction

//        internal DynamicTreeNode(IReference reference)
//        {
//            this.reference = reference;
//        }

//        #endregion

//        #region IHasHandle

//        public IHandle Handle
//        {
//            get
//            {
//                if (Handle == null)
//                {
//                    handle = this.CreateHandle();
//                }
//                return handle;
//            }
//        } private IHandle handle;

//        #endregion

//        #region IReferenceable

//        #region Reference

//        public IReference Reference
//        {
//            get { return reference; }
//            set
//            {
//                if (reference == value) return;
//                if (reference != default(IReference)) throw new NotSupportedException("Reference can only be set once.");
//                reference = value;
//            }
//        } private IReference reference;

//        #endregion

//        #endregion

//        #region IDirectory

//        public IEnumerable<string> ChildrenNames
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public Collections.INotifyingReadOnlyCollection<IHandle> Children
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public IHandle TryGet(string childName)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion




//        public object this[Type type]
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public T AsType<T>() where T : class
//        {
//            throw new NotImplementedException();
//        }

//        public T[] OfType<T>() where T : class
//        {
//            throw new NotImplementedException();
//        }

//        public object[] SubTypes
//        {
//            get { throw new NotImplementedException(); }
//        }


//    }
//}
