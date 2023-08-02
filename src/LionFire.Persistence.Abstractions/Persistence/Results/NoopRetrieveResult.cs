using LionFire.Data;
using LionFire.Data.Gets;

namespace LionFire.Persistence;

public struct NoopRetrieveResult<T> : IGetResult<T>
{
    public static NoopRetrieveResult<T> Instance => new NoopRetrieveResult<T>();

    public bool? IsSuccess => false;

    public T Value => default;

    public bool HasValue => false;

    public TransferResultFlags Flags => TransferResultFlags.Noop | TransferResultFlags.Fail;

    public object? Error => null;

}