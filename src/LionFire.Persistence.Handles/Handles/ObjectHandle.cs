using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
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
        public override Task<IRetrieveResult<ObjectType>> RetrieveImpl()
        {
            if (HasValue)
            {
                return Task.FromResult((IRetrieveResult<ObjectType>)new RetrieveResult<ObjectType>()
                {
                    Flags = PersistenceResultFlags.Success,
                    Value = Object,
                });
            }
            else
            {
                return Task.FromResult((IRetrieveResult<ObjectType>)RetrieveResult<ObjectType>.NotFound);
            }
        }


        #region Construction

        public ObjectHandle() { }
        public ObjectHandle(ObjectType obj) { Object = obj; }
        public ObjectHandle(NamedReference reference, ObjectType obj = null) { Reference = reference; if (obj != null) { Object = obj; } }

        #endregion
    }
}
