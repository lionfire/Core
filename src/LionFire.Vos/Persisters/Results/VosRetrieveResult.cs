using LionFire.Execution;
using LionFire.Referencing;
using LionFire.Vos.Mounts;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Persisters.Vos
{
    public struct VosRetrieveResult<T> : IVosRetrieveResult<T>
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
        public VosRetrieveResult(T value, TransferResultFlags flags) : this(value) { Flags = flags; }

        #endregion

        public object Error { get; set; }
        public T Value { get; set; }
        public bool HasValue =>
            !EqualityComparer<T>.Default.Equals(default, Value);
        //Value != default;

        public TransferResultFlags Flags { get; set; }
        public bool? IsSuccess => Flags.IsSuccessTernary();

        public IReference ResolvedVia => ResolvedViaMount?.Target;
        public IMount ResolvedViaMount { get; set; }
        public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

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

        public static readonly RetrieveResult<T> NotFound = new RetrieveResult<T>()
        {
            Flags = TransferResultFlags.Success | TransferResultFlags.NotFound, // Success but did not find
            Value = default,
        };
        public static readonly RetrieveResult<T> SuccessNotFound = new RetrieveResult<T>()
        {
            Flags = TransferResultFlags.Success | TransferResultFlags.NotFound, // Success but did not find
            Value = default,
        };
        public static readonly RetrieveResult<T> Found = new RetrieveResult<T>()
        {
            Flags = TransferResultFlags.Success | TransferResultFlags.Found, // Success and did find, but did not retrieve Value
            Value = default,
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

        public static readonly RetrieveResult<T> RetrievedNull = new RetrieveResult<T>()
        {
            Flags = TransferResultFlags.Success | TransferResultFlags.Found | TransferResultFlags.RetrievedNullOrDefault,
            Value = default,
        };

        #endregion

        public override bool Equals(object obj) => throw new NotImplementedException();

        public override int GetHashCode() => throw new NotImplementedException();

        public static bool operator ==(VosRetrieveResult<T> left, VosRetrieveResult<T> right) => left.Equals(right);

        public static bool operator !=(VosRetrieveResult<T> left, VosRetrieveResult<T> right) => !(left == right);

        public string ToXamlString() => $"{{VosRetrieveResult {Flags}}}";
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("VosRetrieve ");
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

            if (ResolvedVia != null)
            {
                sb.AppendLine();
                sb.Append(" - ResolvedVia: ");
                sb.Append(ResolvedVia);
            }
            if (Error != null)
            {
                sb.AppendLine();
                sb.Append(" x Error: ");
                sb.Append(Error);
            }

            return sb.ToString();
        }
    }
}
