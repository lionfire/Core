﻿using Hjson;
using System.Text;
using System.Text.Json;
using JsonValue = Hjson.JsonValue;

namespace LionFire.IO.Reactive.Hjson;

public static class HjsonSerialization
{
    public static TValue Deserialize<TValue>(byte[] underlying)
    {
        var hjson = Encoding.UTF8.GetString(underlying);
        var json = HjsonValue.Parse(hjson).ToString(Stringify.Plain);
        return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }

    public static byte[] Serialize<TValue>(TValue usable)
    {
        var json = JsonSerializer.Serialize(usable);
        var hjson = JsonValue.Parse(json).ToString(new HjsonOptions { EmitRootBraces = false });
        return Encoding.UTF8.GetBytes(json);
    }
}

