
using Hjson;

namespace LionFire.Serialization.Hjson_;

public class HjsonSerializerSettings
{

    public HjsonOptions Options { get; set; }

    public HjsonSerializerSettings()
    {
        Options = new HjsonOptions();
    }

}
