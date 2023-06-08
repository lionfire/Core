
using System;
using System.Collections.Generic;
using LionFire.Data.Async.Gets;

namespace LionFire.Persistence
{
    //public struct PersistenceResult<T> : IPersistenceResult 
    //{
    //    #region Construction

    //    //public PersistenceResult()
    //    //{
    //    //    Error = null;
    //    //    Value = default;
    //    //    Flags = default;
    //    //}
    //    public PersistenceResult(T value) : this() { this.Value = value; }
    //    public PersistenceResult(T value, PersistenceResultFlags flags) : this(value) { Flags = flags; }

    //    #endregion

    //    public object Error { get; set; }
    //    public T Value { get; set; }
    //    public bool HasValue =>
    //        !EqualityComparer<T>.Default.Equals(default, Value);
    //    public PersistenceResultFlags Flags { get; set; }
    //    public bool? IsSuccess => Flags.IsSuccessTernary();

    //    public static PersistenceResult<T> NotFoundButInstantiated(T obj) => new PersistenceResult<T>()
    //    {
    //        Flags = PersistenceResultFlags.Success | PersistenceResultFlags.NotFound | PersistenceResultFlags.Instantiated,
    //        Value = obj,
    //    };

    //}
}