#nullable enable

namespace LionFire.Results;

public interface IValueResult { }
public interface IValueResult<out TValue> : IValueResult
{
    TValue? Value { get; }
}

public static class IValueResultX
{

    public static bool HasValue<TValue>(this IValueResult<TValue> v) => v.Value != null;

}