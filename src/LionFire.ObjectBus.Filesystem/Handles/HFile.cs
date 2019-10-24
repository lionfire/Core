//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using LionFire.Referencing;

//namespace LionFire.ObjectBus.Handles
//{

//    public class HOBase<TValue> : WBase<TValue>
//        where TValue : class
//    {
//        public IOBase OBase { get; set; }

//        public override async Task<bool> TryRetrieveObject()
//        {
//            RetrieveInfo ri;
//            await OBase.TryGet(Reference, new OptionalRef<RetrieveInfo>(ri));
//            OnRetrievedObject();
//        }
//        public override Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
//        public override Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
//    }

//    public class HFile<TValue> : WBase<TValue>
//        where TValue : class
//    {
//        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
//        public override Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
//        public override Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
//    }
//}
