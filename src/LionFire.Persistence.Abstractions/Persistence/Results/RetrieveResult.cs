using System;
using LionFire.Referencing;

namespace LionFire.Persistence
{

    public class RetrieveResult<T> : IRetrieveResult<T>
    {
        public object Error { get; set; }
        public T Value { get; set; }
        public bool HasValue => Value != default;

        public PersistenceResultFlags Flags { get; set; }
        public bool? IsSuccess => Flags.IsSuccessTernary();

        #region Static

        public static RetrieveResult<T> Success(T obj) =>  new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success,
            Value = obj,
        };

        public static RetrieveResult<T> Noop(T obj) => new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Noop,
            Value = obj,
        };

        public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound, // Success but did not find
            Value = default,
        };

        public static readonly RetrieveResult<T> InvalidReferenceType = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Fail,
            Value = default,
            Error = "Invalid Reference Type",
        };

        public static readonly RetrieveResult<T> Fail = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Fail,
            Value = default,
        };

        public static readonly RetrieveResult<T> RetrievedNull = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found | PersistenceResultFlags.RetrievedNullOrDefault,
            Value = default,
        };

        #endregion
    }
}