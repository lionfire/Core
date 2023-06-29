
namespace LionFire.Data;


public static class AsyncGetOptions<TValue> 
{
    public static AsyncGetOptions Default { get; set; } = new();
    
    public static IEqualityComparer<TValue> DefaultEqualityComparer  => EqualityComparerOptions<TValue>.Default;

}
public static class AsyncGetOptions<TKey, TValue>
{
    public static AsyncGetOptions Default { get; set; } = new();
}