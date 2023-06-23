﻿#nullable enable
using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct ResolveResultNoop<TValue> : ISuccessResult, ILazyGetResult<TValue>
{
    /// <summary>
    ///  For default values only
    /// </summary>
    public static ResolveResultNoop<TValue> Instance { get; } = new ResolveResultNoop<TValue>();

    public ResolveResultNoop(TValue? value) { Value = value; }

    //public static implicit operator LazyResolveResult<T>(T value) => new LazyResolveResult<T>(values);

    public bool? IsSuccess => true;
    public bool HasValue => true;
    public TValue? Value { get; set; }
    public bool IsNoop => true;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

}

