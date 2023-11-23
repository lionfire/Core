#nullable enable

using LionFire.Serialization;
using System;

namespace LionFire.Persistence.TypeInference;

public class TypeInferenceResult
{
    public Type? Type { get; set; }

    public string? MimeType { get; set; }

    public SerializationFormat? SerializationFormat { get; set; }

}

