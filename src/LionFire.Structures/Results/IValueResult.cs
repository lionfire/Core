#nullable enable

namespace LionFire.Results;

public interface IValueResult { }
public interface IValueResult<out TValue> : IValueResult
{
    TValue? Value { get; }
}
