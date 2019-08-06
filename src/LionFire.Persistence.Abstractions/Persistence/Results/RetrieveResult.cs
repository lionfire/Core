using System;
using LionFire.Referencing;

namespace LionFire.Persistence
{

    public class RetrieveResult<T> : IRetrieveResult<T>
    {
        public object Error { get; set; }
        public T Object { get; set; }

        public PersistenceResultFlags Flags { get; set; }

        #region Static

        public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success,
            Object = default,
        };

        public static readonly RetrieveResult<T> InvalidReferenceType = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Fail,
            Object = default,
            Error = "Invalid Reference Type",
        };

        public static readonly RetrieveResult<T> Fail = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Fail,
            Object = default,
        };

        public static readonly RetrieveResult<T> NullSuccessful = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found,
            Object = default,
        };

        #endregion
    }
}