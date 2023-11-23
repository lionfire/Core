#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using LionFire.Data;
using LionFire.Results;

namespace LionFire.Data;

// TODO: Replace with GetResult<T>?
public struct RetrieveResult<T> : IGetResult<T>, IErrorResult // RENAME to GetResult
//where T : class
{
    #region Static

    public static RetrieveResult<T> Success(T obj) => new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success,
        Value = obj,
    };

    public static RetrieveResult<T> Noop(T obj) => new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Noop,
        Value = obj,
    };

    public static readonly RetrieveResult<T> ProviderNotAvailable = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Fail | TransferResultFlags.ProviderNotAvailable,
        Value = default,
    };
    public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.NotFound, // Success but did not find
        Value = default,
    };
    public static readonly RetrieveResult<T> FailNotFound = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Fail | TransferResultFlags.NotFound, // Fail: did not find
        Value = default,
    };
    public static readonly RetrieveResult<T> SuccessNotFound = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.NotFound, // Success but did not find
        Value = default,
    };
    
    // Noop means this is possibly a redundant/avoidable retrieve, but maybe didn't cost much
    public static readonly RetrieveResult<T> SuccessNotFoundNoop = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.NotFound | TransferResultFlags.Noop, // Success but did not find
        Value = default,
    };
    public static RetrieveResult<T> Found() => found;
    private static readonly RetrieveResult<T> found = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.Found, // Success and did find, but did not retrieve Value
        Value = default,
    };
    public static RetrieveResult<T> Found(T obj) => new RetrieveResult<T>(obj)
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.Found, // Success and did find, but did not retrieve Value
        Value = obj,
    };

    public static readonly RetrieveResult<T> InvalidReferenceType = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Fail,
        Value = default,
        Error = "Invalid Reference Type",
    };

    public static readonly RetrieveResult<T> Fail = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Fail,
        Value = default,
    };

    public static RetrieveResult<T> FromException(Exception ex) => new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Fail,
        Value = default,
        Error = ex,
    };
    public static RetrieveResult<T> NotInitialized { get; } = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Noop | TransferResultFlags.Indeterminate,
        Value = default
    };

    public static readonly RetrieveResult<T> RetrievedNull = new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.Found | TransferResultFlags.RetrievedNullOrDefault,
        Value = default,
    };
    public static RetrieveResult<T> NotFoundButInstantiated(T obj) => new RetrieveResult<T>()
    {
        Flags = TransferResultFlags.Success | TransferResultFlags.NotFound | TransferResultFlags.Instantiated,
        Value = obj,
    };
    #endregion

    #region Construction

    //public RetrieveResult()
    //{
    //    Error = null;
    //    Value = default;
    //    Flags = default;
    //}
    public RetrieveResult(T value) : this() { this.Value = value; }
    public RetrieveResult(T value, TransferResultFlags flags, object? error = null) : this(value) { Flags = flags; Error = error; }

    #endregion

    public object? Error { get; set; }
    public object? ErrorDetail { get; set; }

    public T? Value { get; set; }
    public bool HasValue => !EqualityComparer<T>.Default.Equals(default, Value);
    //Value != default;

    public TransferResultFlags Flags { get; set; }
    public bool? IsSuccess => Flags.IsSuccessTernary();

    public object? InnerResult { get; set; }
    public IEnumerable<IGetResult<T>>? InnerResults { get; set; }

    #region Misc

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(RetrieveResult<T> left, RetrieveResult<T> right) => left.Equals(right);

    public static bool operator !=(RetrieveResult<T> left, RetrieveResult<T> right) => !(left == right);

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("Retrieve ");
        if (IsSuccess != true)
        {
            sb.Append(IsSuccess == null ? "(null IsSuccess)" : (IsSuccess.Value ? "" : "FAIL"));
        }

        sb.Append(" [");
        var flags = Flags.ToDisplayString();
        sb.Append(flags.ToString());

        if (!HasValue && !Flags.HasFlag(TransferResultFlags.NotFound))
        {
            if (flags.Length > 0) sb.Append(", ");
            sb.Append("NOVALUE");
        }
        sb.Append("] ");

        sb.Append(typeof(T).Name);

        if (Error != null)
        {
            sb.AppendLine();
            sb.Append(" - Error: ");
            sb.Append(Error);
        }
        if (ErrorDetail != null)
        {
            sb.AppendLine();
            sb.Append(" - ErrorDetail: ");
            sb.Append(ErrorDetail);
        }

        if (InnerResult != null)
        {
            sb.AppendLine();
            sb.Append(" - Inner result: ");
            sb.Append(InnerResult.ToString());
        }
        return sb.ToString();
    }

    #endregion
}