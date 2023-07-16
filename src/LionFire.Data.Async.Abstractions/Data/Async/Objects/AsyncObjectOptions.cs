
namespace LionFire.Data;

public class AsyncObjectOptions
{
    /// <summary>
    /// Default options to use for child properties.  If null, it will fall back to per-property options (see AsyncValueOptions<TValue>.Default).
    /// </summary>
    public AsyncValueOptions? ValueOptions { get; set; }
    
}