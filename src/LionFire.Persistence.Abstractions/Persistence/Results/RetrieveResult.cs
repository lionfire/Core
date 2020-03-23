
using System;
using System.Collections.Generic;
using LionFire.Referencing;

namespace LionFire.Persistence
{

    public interface IExistsResult : IPersistenceResult
    {
    }

    public struct RetrieveResult<T> : IRetrieveResult<T>
    //where T : class
    {
        #region Construction

        //public RetrieveResult()
        //{
        //    Error = null;
        //    Value = default;
        //    Flags = default;
        //}
        public RetrieveResult(T value) : this() { this.Value = value; }
        public RetrieveResult(T value, PersistenceResultFlags flags) : this(value) { Flags = flags; }

        #endregion

        public object Error { get; set; }
        public T Value { get; set; }
        public bool HasValue =>
            !EqualityComparer<T>.Default.Equals(default, Value);
        //Value != default;

        public PersistenceResultFlags Flags { get; set; }
        public bool? IsSuccess => Flags.IsSuccessTernary();

        public bool IsNoop => Flags.HasFlag(PersistenceResultFlags.Noop);

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
        public static RetrieveResult<T> Found() => found;
        private static readonly RetrieveResult<T> found = new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found, // Success and did find, but did not retrieve Value
            Value = default,
        };
        public static RetrieveResult<T> Found(T obj) => new RetrieveResult<T>(obj)
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Found, // Success and did find, but did not retrieve Value
            Value = obj,
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
        public static RetrieveResult<T> NotFoundButInstantiated(T obj) => new RetrieveResult<T>()
        {
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound | PersistenceResultFlags.Instantiated,
            Value = obj,
        };
        #endregion

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(RetrieveResult<T> left, RetrieveResult<T> right) => left.Equals(right);

        public static bool operator !=(RetrieveResult<T> left, RetrieveResult<T> right) => !(left == right);

    }
}