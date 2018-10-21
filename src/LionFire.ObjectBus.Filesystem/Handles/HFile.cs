using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Handles
{

    public class HOBase<T> : WBase<T>
        where T : class
    {
        public IOBase OBase { get; set; }

        public override Task<bool> TryRetrieveObject()
        {
            RetrieveInfo ri;
            OBase.TryGet(Reference, new OptionalRef<RetrieveInfo>(ri));
            OnRetrievedObject();
        }
        public override Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
        public override Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
    }

    public class HFile<T> : WBase<T>
        where T : class
    {
        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
        public override Task DeleteObject(object persistenceContext = null) => throw new NotImplementedException();
        public override Task WriteObject(object persistenceContext = null) => throw new NotImplementedException();
    }
}
