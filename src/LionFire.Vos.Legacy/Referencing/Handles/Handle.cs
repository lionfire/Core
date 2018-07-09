using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LionFire.Types;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    /// <summary>
    /// FIXME: Is this class useful?  How should the framework resolve handles created dynamically?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Handle<T> : HandleBase<T>
        where T : class//, new()
    {
        public override IReference Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #region Construction

        internal Handle(string uri, T obj = null) : base(uri, obj)
        {
        }

        public Handle(IReference reference, T obj = null) : base(reference, obj)
        {
        }

        public Handle(IReferencable referencable, T obj = null) : base(referencable, obj)
        {
        }

        public Handle(T obj = null, bool freezeObjectIfProvided = true) : base(obj, freezeObjectIfProvided)
        {
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            throw new NotImplementedException();
        }

        public override Task Save(object persistenceContext = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
