using LionFire.Data;
using LionFire.Data.Async.Sets;

namespace LionFire.Persistence;

public struct NoopFailDeleteResult<TValue> : IDeleteResult
{
    public static readonly NoopFailDeleteResult<TValue> Instance = new NoopFailDeleteResult<TValue>();

    public bool? IsSuccess => false;

    public TValue Value => default;
    public bool HasValue => false;

    public TransferResultFlags Flags { get => TransferResultFlags.Fail | TransferResultFlags.Noop; }
}
