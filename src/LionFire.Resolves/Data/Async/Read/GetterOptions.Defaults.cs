
namespace LionFire.Data.Async.Gets;


public static class GetterOptions<TValue> 
{
    public static GetterOptions Default { get; set; } = new();
    
    public static IEqualityComparer<TValue> DefaultEqualityComparer  => EqualityComparerOptions<TValue>.Default;

}
public static class GetterOptions<TKey, TValue>
{
    public static GetterOptions Default { get; set; } = new();
}