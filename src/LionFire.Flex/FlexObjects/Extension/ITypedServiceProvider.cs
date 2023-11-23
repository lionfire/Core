
namespace LionFire.FlexObjects;

public interface ITypedObjectProvider
{
    T? Query<T>(string? key = null);
    object? Query(Type type, string? key = null); // TODO: out var result, return bool
}