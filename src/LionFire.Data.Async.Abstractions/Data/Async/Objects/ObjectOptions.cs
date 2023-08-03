
namespace LionFire.Data.Async;

public class ObjectOptions
{
    /// <summary>
    /// Default options to use for child properties.  If null, it will fall back to per-property options (see AsyncValueOptions<TValue>.Default).
    /// </summary>
    public ValueOptions? ValueOptions { get; set; }
    
}