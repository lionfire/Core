using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Persisters.Vos
{
    public struct VosRetrieveResult<T> :  IVosRetrieveResult<T>
    {
        // Largely a DUPLICATE of RetrieveResult<T>

        public IReadHandleBase<T> ReadHandle { get; set; }

        #region Construction

        //public RetrieveResult()
        //{
        //    Error = null;
        //    Value = default;
        //    Flags = default;
        //}
        public VosRetrieveResult(T value) : this() { this.Value = value; }
        public VosRetrieveResult(T value, PersistenceResultFlags flags) : this(value) { Flags = flags; }

        #endregion

        public object Error { get; set; }
        public T Value { get; set; }
        public bool HasValue =>
            !EqualityComparer<T>.Default.Equals(default, Value);
        //Value != default;

        public PersistenceResultFlags Flags { get; set; }
        public bool? IsSuccess => Flags.IsSuccessTernary();

        #region Static

        public static RetrieveResult<T> Success(T obj) => new RetrieveResult<T>()
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
        public static readonly RetrieveResult<T> SuccessNotFound = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound, // Success but did not find
            Value = default,
        };
        public static readonly RetrieveResult<T> Found = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found, // Success and did find, but did not retrieve Value
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

        public override bool Equals(object obj) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();

        public static bool operator ==(VosRetrieveResult<T> left, VosRetrieveResult<T> right) => left.Equals(right);

        public static bool operator !=(VosRetrieveResult<T> left, VosRetrieveResult<T> right) => !(left == right);

        public override string ToString() => $"{{VosRetrieveResult {Flags.ToString()}}}";
    }
}
