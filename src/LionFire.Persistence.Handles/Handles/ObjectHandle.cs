using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// REVIEW: Incomplete? / needs design analysis
    /// Reference type: NamedReference
    /// Read-only Handles to .NET object references (can be null) -- can be named and then retrieved by the handle registry system.
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public class ObjectHandle<ObjectType> : RBaseEx<ObjectType>, IKeyed<string>
        where ObjectType : class
    {
        public override IEnumerable<Type> AllowedReferenceTypes
        {
            get
            {
                yield return typeof(NamedReference);
            }
        }
        public override Task<IResolveResult<ObjectType>> ResolveImpl()
        {
            if (HasValue)
            {
                return Task.FromResult<IResolveResult<ObjectType>>(new RetrieveResult<ObjectType>()
                {
                    Flags = PersistenceResultFlags.Success,
                    Value = Value,
                });
            }
            else
            {
                return Task.FromResult<IResolveResult<ObjectType>>(RetrieveResult<ObjectType>.NotFound);
            }
        }


        #region Construction

        public ObjectHandle() { }
        public ObjectHandle(ObjectType obj) { Value = obj; }
        public ObjectHandle(NamedReference reference, ObjectType obj = null) { Reference = reference; if (obj != null) { Value = obj; } }

        #endregion
    }
}
