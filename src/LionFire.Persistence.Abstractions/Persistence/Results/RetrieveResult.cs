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

        public static RetrieveResult<T> Success(T obj) =>  new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success,
            Object = obj,
        };

        public static RetrieveResult<T> Noop(T obj) => new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Noop,
            Object = obj,
        };

        public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound, // Success but did not find
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

        public static readonly RetrieveResult<T> RetrievedNull = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found | PersistenceResultFlags.RetrievedNull,
            Object = default,
        };

        #endregion
    }
}