using LionFire.Ontology;
using System;
using System.Net.WebSockets;
using System.Text;

namespace LionFire.Data;

// TODO: sort out nomenclature for Read vs Retrieve, propagate these interfaces to the right places

// TODO: Reorg the numbers, group for Resolve/Transmit/Persist.
[Flags]
public enum TransferResultFlags // TODO RENAME GLOBALRENAME to TransferResultFlags
{
    None = 0,

    /// <summary>
    /// True if operation was successful, even if retrieve did not find anything
    /// </summary>
    Success = 1 << 0,

    /// <summary>
    /// Exception
    /// </summary>
    Fail = 1 << 1, // RENAME to exception?

    Found = 1 << 2,

    NotFound = 1 << 3,

    Retrieved = 1 << 4,

    /// <summary>
    /// Retrieved null (for reference types) or default value (for value types)
    /// </summary>
    RetrievedNullOrDefault = 1 << 5,

    Indeterminate = 1 << 6,

    // = 1 << 7,
    // = 1 << 8,
    // = 1 << 9,

    /// <summary>
    /// For operations that result in instantiating a value in memory
    /// </summary>
    Instantiated = 1 << 10,

    /// <summary>
    /// Indicate a value was created in the underlying data store (REVIEW)
    /// </summary>
    Created = 1 << 11,

    // = 1 << 12,
    // = 1 << 12,
    // = 1 << 13,
    // = 1 << 14,

    /// <summary>
    /// Failure in resolving a route from the source address to the ultimate target, such as:
    ///  - No provider registered for the URI scheme
    ///  - Failure to parse an address
    /// </summary>
    ResolveFail = 1 << 15, // TODO: Implement

    /// <summary>
    /// Failure in transit, such as a network failure, or lack of a serialization mechanism for transmitting the data.
    /// </summary>
    TransitFail = 1 << 16, // TODO: Implement

    /// <summary>
    /// Failure at the target, such as:
    ///  - a missing connection string for the desired database,
    ///  - a hard disk failure, 
    ///  - lack of a serialization mechanism for saving or loading the data.
    /// </summary>
    TargetFail = 1 << 17, // TODO: Implement

    // = 1 << 18,
    // = 1 << 19,

    /// <summary>
    /// When checking for whether an operation is possible, this is set if the operation is expected to succeed.
    /// </summary>
    PreviewSuccess = 1 << 20,

    /// <summary>
    /// When checking for whether an operation is possible, this is set if the operation is expected to fail.
    /// </summary>
    PreviewFail = 1 << 21,

    PreviewIndeterminate = 1 << 22,

    //PreviewNotFound = 1 << 23, // Not used yet. Should it be?

    // 1 << 24

    Ambiguous = 1 << 25,

    InnerFail = 1 << 26, // MOVE number near Fail?

    /// <summary>
    /// The database (or other low-level persister for saving or loading data) is not available at the source.
    /// </summary>
    PersisterNotAvailable = 1 << 27,

    /// <summary>
    /// The provider (high-level addressing mechanism for saving or loading data) is not available at the invokation point.
    /// If this is set, ResolveFail should also be set.
    /// </summary>
    ProviderNotAvailable = 1 << 28, // RENAME: SourceProviderNotAvailable

    /// <summary>
    /// The mount point for saving or loading data is not mounted at the specified address.
    /// </summary>
    MountNotAvailable = 1 << 29,

    /// <summary>
    /// A serializer is not available for the get or set operation.
    /// This could be for data in transit (check the TransitFail flag), 
    /// or for data at rest or final destination/source (check the TargetFail flag).
    /// </summary>
    SerializerNotAvailable = 1 << 30,

    Noop = 1 << 31,
}

public static class TransferResultFlagsExtensions
{
    public static string ToDisplayString(this TransferResultFlags flags, bool uppercase = true)
    {
        var sb = new StringBuilder();
        bool noSuccess = !flags.HasFlag(TransferResultFlags.Success) && !flags.HasFlag(TransferResultFlags.Fail);

        flags &= ~TransferResultFlags.Success;

        if (flags != TransferResultFlags.None)
        {
            sb.Append(flags.ToString());
        }

        if (noSuccess)
        {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append("NoSuccess");
        }

        if (sb.Length == 0) sb.Append("NoFlags");
        var result = sb.ToString();
        return uppercase ? result.ToUpperInvariant() : result;
    }

    public static bool? IsFound(this TransferResultFlags flags)
    {
        if (flags.HasFlag(TransferResultFlags.Found)) return true;
        if (flags.HasFlag(TransferResultFlags.NotFound)) return false;
        return null;
    }

    public static PersistenceResult ToResult(this TransferResultFlags flags) => new PersistenceResult { Flags = flags };
}