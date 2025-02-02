using LionFire.Structures;

namespace LionFire.Schemas;

public class Schema : IKeyed<string>
{

    public string Key { get; set; }

    public Schema(string key)
    {
        Key = key;
    }
}

