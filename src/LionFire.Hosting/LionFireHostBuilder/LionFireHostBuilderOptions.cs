#if UNUSED
namespace LionFire.Hosting;

public enum LionFireDefaults : uint
{
    None = 0,

    Logging = 1 << 0,

    All = 0xFFFFFFFF,
}

public class LionFireHostBuilderOptions
{
    public LionFireDefaults Defaults { get; set; }
}
#endif