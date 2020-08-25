#if Experimental
using LionFire.Dependencies;
using LionFire.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public class LionFireTypeJsonWriter : JsonWriter
    {
        private readonly string _fieldName;
        private readonly string _disc;
        private bool _discriminatorWritten;
        private readonly JsonWriter writer;


        public LionFireTypeJsonWriter(string fieldName, string disc, JsonWriter writer)
        {
            _discriminatorWritten = false;
            _fieldName = fieldName;
            _disc = disc;
            this.writer = writer;
        }


        public override void Flush() => writer.Flush();

        public override bool Equals(object obj) => writer.Equals(obj);
        public override int GetHashCode() => writer.GetHashCode();
        public override string ToString() => writer.ToString();
        public override Task CloseAsync(CancellationToken cancellationToken = default) => writer.CloseAsync(cancellationToken);
        public override Task FlushAsync(CancellationToken cancellationToken = default) => writer.FlushAsync(cancellationToken);
        protected override Task WriteEndAsync(JsonToken token, CancellationToken cancellationToken) => writer.WriteEndAsync(token, cancellationToken);
        protected override Task WriteIndentAsync(CancellationToken cancellationToken) => writer.WriteIndentAsync(cancellationToken);
        protected override Task WriteValueDelimiterAsync(CancellationToken cancellationToken) => writer.WriteValueDelimiterAsync(cancellationToken);
        protected override Task WriteIndentSpaceAsync(CancellationToken cancellationToken) => writer.WriteIndentSpaceAsync(cancellationToken);
        public override Task WriteRawAsync(string json, CancellationToken cancellationToken = default) => writer.WriteRawAsync(json, cancellationToken);
        public override Task WriteEndAsync(CancellationToken cancellationToken = default) => writer.WriteEndAsync(cancellationToken);
        public override Task WriteEndArrayAsync(CancellationToken cancellationToken = default) => writer.WriteEndArrayAsync(cancellationToken);
        public override Task WriteEndConstructorAsync(CancellationToken cancellationToken = default) => writer.WriteEndConstructorAsync(cancellationToken);
        public override Task WriteEndObjectAsync(CancellationToken cancellationToken = default) => writer.WriteEndObjectAsync(cancellationToken);
        public override Task WriteNullAsync(CancellationToken cancellationToken = default) => writer.WriteNullAsync(cancellationToken);
        public override Task WritePropertyNameAsync(string name, CancellationToken cancellationToken = default) => writer.WritePropertyNameAsync(name, cancellationToken);
        public override Task WritePropertyNameAsync(string name, bool escape, CancellationToken cancellationToken = default) => writer.WritePropertyNameAsync(name, escape, cancellationToken);
        public override Task WriteStartArrayAsync(CancellationToken cancellationToken = default) => writer.WriteStartArrayAsync(cancellationToken);
        public override Task WriteCommentAsync(string text, CancellationToken cancellationToken = default) => writer.WriteCommentAsync(text, cancellationToken);
        public override Task WriteRawValueAsync(string json, CancellationToken cancellationToken = default) => writer.WriteRawValueAsync(json, cancellationToken);
        public override Task WriteStartConstructorAsync(string name, CancellationToken cancellationToken = default) => writer.WriteStartConstructorAsync(name, cancellationToken);
        public override Task WriteStartObjectAsync(CancellationToken cancellationToken = default) => writer.WriteStartObjectAsync(cancellationToken);
        public override Task WriteValueAsync(bool value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(bool? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(byte value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(byte? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(byte[] value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(char value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(char? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(DateTime value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(DateTime? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(DateTimeOffset value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(DateTimeOffset? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(decimal value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(decimal? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(double value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(double? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(float value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(float? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(Guid value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(Guid? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(int value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(int? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(long value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(long? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(object value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(sbyte value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(sbyte? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(short value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(short? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(string value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(TimeSpan value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(TimeSpan? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(uint value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(uint? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(ulong value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(ulong? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(Uri value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(ushort value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteValueAsync(ushort? value, CancellationToken cancellationToken = default) => writer.WriteValueAsync(value, cancellationToken);
        public override Task WriteUndefinedAsync(CancellationToken cancellationToken = default) => writer.WriteUndefinedAsync(cancellationToken);
        public override Task WriteWhitespaceAsync(string ws, CancellationToken cancellationToken = default) => writer.WriteWhitespaceAsync(ws, cancellationToken);
        public override void Close() => writer.Close();
        public override void WriteStartObject() => writer.WriteStartObject();
        public override void WriteEndObject() => writer.WriteEndObject();
        public override void WriteStartArray() => writer.WriteStartArray();
        public override void WriteEndArray() => writer.WriteEndArray();
        public override void WriteStartConstructor(string name) => writer.WriteStartConstructor(name);
        public override void WriteEndConstructor() => writer.WriteEndConstructor();
        public override void WritePropertyName(string name) => writer.WritePropertyName(name);
        public override void WritePropertyName(string name, bool escape) => writer.WritePropertyName(name, escape);
        public override void WriteEnd() => writer.WriteEnd();
        protected override void WriteEnd(JsonToken token) => writer.WriteEnd(token);
        protected override void WriteIndent() => writer.WriteIndent();
        protected override void WriteValueDelimiter() => writer.WriteValueDelimiter();
        protected override void WriteIndentSpace() => writer.WriteIndentSpace();
        public override void WriteNull() => writer.WriteNull();
        public override void WriteUndefined() => writer.WriteUndefined();
        public override void WriteRaw(string json) => writer.WriteRaw(json);
        public override void WriteRawValue(string json) => writer.WriteRawValue(json);
        public override void WriteValue(string value) => writer.WriteValue(value);
        public override void WriteValue(int value) => writer.WriteValue(value);
        public override void WriteValue(uint value) => writer.WriteValue(value);
        public override void WriteValue(long value) => writer.WriteValue(value);
        public override void WriteValue(ulong value) => writer.WriteValue(value);
        public override void WriteValue(float value) => writer.WriteValue(value);
        public override void WriteValue(double value) => writer.WriteValue(value);
        public override void WriteValue(bool value) => writer.WriteValue(value);
        public override void WriteValue(short value) => writer.WriteValue(value);
        public override void WriteValue(ushort value) => writer.WriteValue(value);
        public override void WriteValue(char value) => writer.WriteValue(value);
        public override void WriteValue(byte value) => writer.WriteValue(value);
        public override void WriteValue(sbyte value) => writer.WriteValue(value);
        public override void WriteValue(decimal value) => writer.WriteValue(value);
        public override void WriteValue(DateTime value) => writer.WriteValue(value);
        public override void WriteValue(DateTimeOffset value) => writer.WriteValue(value);
        public override void WriteValue(Guid value) => writer.WriteValue(value);
        public override void WriteValue(TimeSpan value) => writer.WriteValue(value);
        public override void WriteValue(int? value) => writer.WriteValue(value);
        public override void WriteValue(uint? value) => writer.WriteValue(value);
        public override void WriteValue(long? value) => writer.WriteValue(value);
        public override void WriteValue(ulong? value) => writer.WriteValue(value);
        public override void WriteValue(float? value) => writer.WriteValue(value);
        public override void WriteValue(double? value) => writer.WriteValue(value);
        public override void WriteValue(bool? value) => writer.WriteValue(value);
        public override void WriteValue(short? value) => writer.WriteValue(value);
        public override void WriteValue(ushort? value) => writer.WriteValue(value);
        public override void WriteValue(char? value) => writer.WriteValue(value);
        public override void WriteValue(byte? value) => writer.WriteValue(value);
        public override void WriteValue(sbyte? value) => writer.WriteValue(value);
        public override void WriteValue(decimal? value) => writer.WriteValue(value);
        public override void WriteValue(DateTime? value) => writer.WriteValue(value);
        public override void WriteValue(DateTimeOffset? value) => writer.WriteValue(value);
        public override void WriteValue(Guid? value) => writer.WriteValue(value);
        public override void WriteValue(TimeSpan? value) => writer.WriteValue(value);
        public override void WriteValue(byte[] value) => writer.WriteValue(value);
        public override void WriteValue(Uri value) => writer.WriteValue(value);
        public override void WriteValue(object value) => writer.WriteValue(value);
        public override void WriteComment(string text) => writer.WriteComment(text);
        public override void WriteWhitespace(string ws) => writer.WriteWhitespace(ws);
        protected override void Dispose(bool disposing) => writer.Dispose(disposing);
    }
    public class LionFireJsonConverter : JsonConverter
    {
        public static TypeNameRegistry TypeNameRegistry => typeNameRegistry ??= DependencyContext.Default.GetService<TypeNameRegistry>();
        private static TypeNameRegistry typeNameRegistry;

        public string TypePropertyName = "$type";

        public override bool CanConvert(Type objectType) => true;
        //public override bool CanRead { get => !_isInRead.Value; }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObject = JObject.Load(reader);

            var discriminator = jObject[TypePropertyName]?.Value<string>();
            if (!string.IsNullOrEmpty(discriminator))
            {
                base.ReadJson
            }

        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var objectType = value.GetType();

        }
    }

    public class JsonKnownTypesConverter<T> : JsonConverter
    {
        private readonly DiscriminatorValues _typesDiscriminatorValues
            = JsonKnownTypesSettingsManager.GetDiscriminatorValues<T>();

        public override bool CanConvert(Type objectType)
            => _typesDiscriminatorValues.Contains(objectType);

        private readonly ThreadLocal<bool> _isInRead = new ThreadLocal<bool>();

        public override bool CanRead { get => !_isInRead.Value; }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObject = JObject.Load(reader);

            var discriminator = jObject[_typesDiscriminatorValues.FieldName]?.Value<string>();

            if (string.IsNullOrEmpty(discriminator))
                throw new JsonKnownTypesException($"There is no discriminator fields with {_typesDiscriminatorValues.FieldName} name in json string or it is empty.");

            if (_typesDiscriminatorValues.TryGetType(discriminator, out var typeForObject))
            {
                var jsonReader = jObject.CreateReader();

                _isInRead.Value = true;

                var obj = serializer.Deserialize(jsonReader, typeForObject);

                _isInRead.Value = false;

                return obj;
            }

            throw new JsonKnownTypesException($"{discriminator} discriminator is not registered for {nameof(T)} type");
        }

        private readonly ThreadLocal<bool> _isInWrite = new ThreadLocal<bool>();

        public override bool CanWrite { get => !_isInWrite.Value; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var objectType = value.GetType();
            if (_typesDiscriminatorValues.TryGetDiscriminator(objectType, out var discriminator))
            {
                _isInWrite.Value = true;

                var writerProxy = new JsonKnownProxyWriter(_typesDiscriminatorValues.FieldName, discriminator, writer);
                serializer.Serialize(writerProxy, value, objectType);

                _isInWrite.Value = false;
            }
            else
            {
                throw new JsonKnownTypesException($"There is no discriminator for {objectType.Name} type");
            }
        }
    }
}

#endif