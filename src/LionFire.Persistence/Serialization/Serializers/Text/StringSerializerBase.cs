using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{

    public abstract class StringSerializerBase<TConcrete> : SerializerBase<TConcrete>
        where TConcrete : StringSerializerBase<TConcrete>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize;

        public override DeserializationResult<T> ToObject<T>(string str, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (typeof(T) != typeof(string)) return DeserializationResult<T>.NotSupported;
            return (T)(object)str; // HARDCAST
        }

        public override DeserializationResult<T> ToObject<T>(byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            if (typeof(T) != typeof(string)) return DeserializationResult<T>.NotSupported;
            return (T)(object)BytesToString(bytes, context); // HARDCAST  
        }

        public override DeserializationResult<T> ToObject<T>(Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
        {
            var bytes = new byte[stream.Length - stream.Position];
            stream.Read(bytes, 0, bytes.Length);
            return ToObject<T>(BytesToString(bytes, context), operation, context);
        }

        public override (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => obj is string s ? (s, SerializationResult.Success) : ((string String, SerializationResult Result))(default, SerializationResult.NotSupported);

        public override (byte[] Bytes, SerializationResult Result) ToBytes(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => obj is string s ? (StringToBytes(s, context), SerializationResult.Success) : ((byte[] Bytes, SerializationResult Result))(default, SerializationResult.NotSupported);

        //public override SerializationResult ToStream(object obj, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => base.ToStream(obj, stream, operation, context);

        #region Scoring

        public override IEnumerable<ISerializeScorer> SerializeScorers
        {
            get
            {
                yield return TextTypeScorer;
            }
        }
        public override IEnumerable<IDeserializeScorer> DeserializeScorers
        {
            get
            {
                yield return TextTypeScorer;
            }
        }

        public virtual TextTypeScorer TextTypeScorer => Singleton<TextTypeScorer>.Instance;

        #endregion

    }

    public class TextTypeScorer : ISerializeScorer, IDeserializeScorer
    {
        public float PassScore { get; set; } = 10f;
        public float FailScore { get; set; } = -10f;

        public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => operation.Value.Type == typeof(string) ? PassScore : FailScore;
    }
}