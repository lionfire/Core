using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    /// Freeze behavior: After frozen, Reference can only be changed from null to non-null.  No other restrictions.
    /// FIXME: Is this class useful?  How should the framework resolve handles created dynamically?
    /// </summary>
    public class Handle : HandleBase<object>
    {
        public override IReference Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #region Construction

        internal Handle(string uri, object obj = null)
            : base(uri, obj)
        {
        }

        public Handle(IReference reference, object obj = null)
            : base(reference, obj)
        {
        }

        public Handle(IReferencable referencable, object obj = null)
            : base(referencable, obj)
        {
        }

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            throw new NotImplementedException();
        }

        public override Task Commit(object persistenceContext = null)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
