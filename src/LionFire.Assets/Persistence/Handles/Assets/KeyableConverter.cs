using LionFire.Structures;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LionFire.Assets
{
    public class KeyableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(IKeyable<string>).IsAssignableFrom(objectType) && !objectType.IsInterface && !objectType.IsAbstract;

        //static ConcurrentDictionary<Type, MethodInfo> FromKeyMethods { get; } = new ConcurrentDictionary<Type, MethodInfo>();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = (IKeyable<string>)existingValue;
#if DEBUG
            if (result != null && result.Key != reader.Value as string)
            {
                Debug.WriteLine($"[deserialize] {this.GetType().Name} overwriting existing object of type {existingValue.GetType().Name}: {result.Key} => {reader.Value}");
            }
#endif

            //result ??= (IKeyable<string>)Activator.CreateInstance(objectType);
            result = (IKeyable<string>)Activator.CreateInstance(objectType);

            result.Key = reader.Value as string;
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IKeyed<string>)value).Key);
        }
    }
}
