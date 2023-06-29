
using System;
using System.Collections.Generic;
using LionFire.Data.Gets;

namespace LionFire.Persistence
{
    //public struct TransferResult<T> : ITransferResult 
    //{
    //    #region Construction

    //    //public TransferResult()
    //    //{
    //    //    Error = null;
    //    //    Value = default;
    //    //    Flags = default;
    //    //}
    //    public TransferResult(T value) : this() { this.Value = value; }
    //    public TransferResult(T value, TransferResultFlags flags) : this(value) { Flags = flags; }

    //    #endregion

    //    public object Error { get; set; }
    //    public T Value { get; set; }
    //    public bool HasValue =>
    //        !EqualityComparer<T>.Default.Equals(default, Value);
    //    public TransferResultFlags Flags { get; set; }
    //    public bool? IsSuccess => Flags.IsSuccessTernary();

    //    public static TransferResult<T> NotFoundButInstantiated(T obj) => new TransferResult<T>()
    //    {
    //        Flags = TransferResultFlags.Success | TransferResultFlags.NotFound | TransferResultFlags.Instantiated,
    //        Value = obj,
    //    };

    //}
}