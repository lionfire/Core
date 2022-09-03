using System;

namespace LionFire.Persistence.TypeInference;

[Flags]
public enum TypeInferenceMode
{
    Unspecified = 0,

    Deserialize = 1 << 0,

    /// <summary>
    /// First few bytes of the serializd stream
    /// </summary>
    Magic = 1 << 1,

    /// <summary>
    /// (File) extension on the key, such as zip, txt, etc.
    /// </summary>
    Extension = 1 << 2,

    /// <summary>
    /// Present as OOB data, such as a database column name
    /// </summary>
    Attribute = 1 << 3,
}

