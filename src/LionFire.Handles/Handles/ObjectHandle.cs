using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// REVIEW: Incomplete? / needs design analysis
    /// Reference type: NamedReference
    /// Read-only Handles to .NET object references (can be null) -- can be named and then retrieved by the handle registry system.
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public class ObjectHandle<ObjectType> : RBase<ObjectType>, IKeyed<string>
        where ObjectType : class
    {
        public override IEnumerable<Type> AllowedReferenceTypes
        {
            get
            {
                yield return typeof(NamedReference);
            }
        }
        public override Task<bool> TryRetrieveObject() => Task.FromResult(false);

        #region Construction

        public ObjectHandle() { }
        public ObjectHandle(ObjectType obj) { Object = obj; }
        public ObjectHandle(NamedReference reference, ObjectType obj = null) { Reference = reference; if (obj != null) { Object = obj; } }

        #endregion
    }
}
