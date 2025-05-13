using DynamicData;
using Hjson;
using LionFire.Serialization;
using LionFire.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Linq;
using JsonValue = Hjson.JsonValue;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace LionFire.IO.Reactive.Hjson;

public static class HjsonSerialization
{
    static JsonSerializerSettings Settings
    {
        get
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = [new SourceCacheConverter(), new ParsableSlimConverter()],
                    //ContractResolver = new CustomSystemTextJsonIgnoreResolver(),

                    //ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.ContractResolver = Singleton<IgnoreDataContractContractResolver>.Instance; // MEMORY: permanently increases with seen types

                //settings.ContractResolver = new DefaultContractResolver
                //{
                //    NamingStrategy = new DefaultNamingStrategy()
                //    //NamingStrategy = new CamelCaseNamingStrategy
                //    //{
                //    //    ProcessDictionaryKeys = true,
                //    //    OverrideSpecifiedNames = false
                //    //}
                //};
            }
            return settings;
        }
    }

    static JsonSerializerSettings? settings;

    public static TValue Deserialize<TValue>(byte[] underlying)
    {
        var hjson = Encoding.UTF8.GetString(underlying);
        var json = HjsonValue.Parse(hjson).ToString(Stringify.Plain);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(json
            , Settings
            )!;
        //return JsonSerializer.Deserialize<TValue>(json) ?? throw new NotSupportedException("Deserializing null not supported");
    }

    public static byte[] Serialize<TValue>(TValue usable)
    {
        //var json = JsonSerializer.Serialize(usable);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(usable, Settings);
        var hjson = JsonValue.Parse(json).ToString(new HjsonOptions { EmitRootBraces = false });
        return Encoding.UTF8.GetBytes(hjson);
    }
}

public static class IObservableCacheX
{
    public static object UnwrapValueTuple(object oc, Type keyType, Type valueTupleType)
    {
        var valueType = valueTupleType.GetGenericArguments()[1];

        var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dict = Activator.CreateInstance(dictType)!;
        var addMethod = dictType.GetMethod("Add", new[] { keyType, valueType })!;
        var itemsProperty = oc.GetType().GetProperty("KeyValues")!;
        var items = (System.Collections.IEnumerable) itemsProperty.GetValue(oc)!;

        //var valueTupleType = typeof(ValueTuple<,>).MakeGenericType(keyType, valueType);
        var valueItem2 = valueTupleType.GetField("Item2")!;

        foreach (var item in items)
        {
            var key = item.GetType().GetProperty("Key")!.GetValue(item);
            var valueTuple = item.GetType().GetProperty("Value")!.GetValue(item);

            var value = valueItem2.GetValue(valueTuple);

            addMethod.Invoke(dict, [key, value]);
        }
        return dict;
    }

    public static IReadOnlyDictionary<TKey, TValue> UnwrapValueTuple<TKey, TValue>(this IObservableCache<(TKey, TValue), TKey> oc)
        where TKey : notnull
        where TValue : notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in oc.KeyValues)
        {
            var key = item.Key;
            var val = item.Value.Item2;
            dict.Add(key, val);
        }
        return dict;
    }

    public static SourceCache<(TKey, TValue), TKey> ToKeyTaggedSourceCache<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict)
        where TKey : notnull
        where TValue : notnull
    {
        var cache = new SourceCache<(TKey, TValue), TKey>(x => x.Item1);
        cache.Edit(u =>
        {
            u.AddOrUpdate(dict.Select(kvp => (kvp.Key, kvp.Value)));
            //foreach (var kvp in dict)
            //{
            //    var tuple = (kvp.Key, kvp.Value);
            //    u.AddOrUpdate(tuple);
            //}
        });
        return cache;
    }

}


public class SourceCacheConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // Check if the type is a SourceCache<,> (any TKey, TValue)
        var r = objectType.IsGenericType
            && objectType.GetGenericTypeDefinition() == typeof(SourceCache<,>);
        if (!r) return false;

        var valueType = objectType.GetGenericArguments()[0];

        // valueType must be ValueTuple<TKey,>
        return valueType.IsGenericType
            && valueType.GetGenericTypeDefinition() == typeof(ValueTuple<,>)
            && valueType.GetGenericArguments()[0] == objectType.GetGenericArguments()[1] // TKey
            ;
    }


    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        // Get the generic type arguments
        var sourceCacheType = value.GetType();
        var valueType = sourceCacheType.GetGenericArguments()[0];
        var keyType = sourceCacheType.GetGenericArguments()[1];

        //// Access the Items property to get key-value pairs
        //var keyValuesProperty = sourceCacheType.GetProperty("KeyValues");
        //if (keyValuesProperty == null)
        //{
        //    throw new JsonSerializationException("Unable to access Items property on SourceCache.");
        //}

        //// Get the IEnumerable<KeyValuePair<TKey, TValue>>
        //var dictionary = keyValuesProperty.GetValue(value);

        var dictionary = IObservableCacheX.UnwrapValueTuple(value, keyType, valueType);

        //if (dictionary == null)
        //{
        //    writer.WriteNull();
        //    return;
        //}

        //// Convert to dictionary using reflection
        //var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        //var dictionary = Activator.CreateInstance(dictionaryType);

        //// Use LINQ to populate dictionary (ToDictionary equivalent)
        //var kvpType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
        //var keyProperty = kvpType.GetProperty("Key");
        //var valueProperty = kvpType.GetProperty("Value");
        //var addMethod = dictionaryType.GetMethod("Add");

        //foreach (var item in (IEnumerable<object>)dictionary)
        //{
        //    var key = keyProperty.GetValue(item);
        //    var valueTuple = valueProperty.GetValue(item);
        //    addMethod.Invoke(dictionary, new[] { key, valueTuple });
        //}

        // Serialize the dictionary
        //serializer.Serialize(writer, dictionary);
        serializer.Serialize(writer, dictionary);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) { return null; }

        var valueTupleType = objectType.GetGenericArguments()[0];
        var valueType = valueTupleType.GetGenericArguments()[1];
        var keyType = objectType.GetGenericArguments()[1];

        // Deserialize into a Dictionary<TKey, TValue>
        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dictionary = serializer.Deserialize(reader, dictionaryType);
        if (dictionary == null) { return null; }

        var toKeyTaggedSourceCacheMethod = typeof(IObservableCacheX)
            .GetMethod(nameof(IObservableCacheX.ToKeyTaggedSourceCache), BindingFlags.Static | BindingFlags.Public)
            ?.MakeGenericMethod(keyType, valueType)!;
        var cache = toKeyTaggedSourceCacheMethod.Invoke(null, [dictionary]);

        return cache;
    }

    private static object CreateKeySelector(Type keyType, Type valueType)
    {
        // Use Linq to create a key selector for a ValueTuple<TKey, TValue>
        var keySelectorType = typeof(Func<,>).MakeGenericType(valueType, keyType);
        var keySelector = Expression.Lambda(
            Expression.Property(Expression.Parameter(valueType, "x"), "Item1"),
            Expression.Parameter(valueType, "x")
        );
        return keySelector.Compile();
    }
}


public class ParsableSlimConverter : JsonConverter
{
    
    public override bool CanConvert(Type objectType)
    {
        // Check if the type implements IParsableSlim<T> for some T
        if (objectType.FullName!.Contains("OptimizationRunRef"))
        {
            Debug.WriteLine($"ORR - {Array.Exists(objectType.GetInterfaces(), i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsableSlim<>))}");
        }
        return Array.Exists(objectType.GetInterfaces(), i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsableSlim<>));
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        string? value = reader.Value?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        // Find the Parse method on the type
        MethodInfo parseMethod = objectType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null)!;
        if (parseMethod == null)
        {
            throw new JsonSerializationException($"Type {objectType.Name} does not have a public static Parse(string) method.");
        }

        return parseMethod.Invoke(null, new[] { value });
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.ToString());
    }
}