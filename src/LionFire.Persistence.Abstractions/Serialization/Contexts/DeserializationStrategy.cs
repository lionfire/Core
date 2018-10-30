using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization
{
    public enum DeserializationStrategy
    {
        Unspecified,
        FileName = 1 << 0,
        MimeType = 1 << 1,
        Headers = 1 << 2,
        Detect = 1 << 3,
    }
}
