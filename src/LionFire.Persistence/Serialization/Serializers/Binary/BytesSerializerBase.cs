using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Serialization
{

    public abstract class BytesSerializerBase<TConcrete> : SerializerBase<TConcrete>
        where TConcrete : BytesSerializerBase<TConcrete>
    {
        public override SerializationFlags SupportedCapabilities =>
            SerializationFlags.Binary
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize;

        public override DeserializationResult<T> ToObject<T>(byte[] bytes, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => typeof(T) != typeof(byte[])
            ? DeserializationResult<T>.NotSupported
            : (T)(object)bytes; // HARDCAST

        public override (byte[] Bytes, SerializationResult Result) ToBytes(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => obj is byte[] bytes ? (bytes, SerializationResult.Success) : ((byte[] Bytes, SerializationResult Result))(default, SerializationResult.NotSupported);

        #region Scoring

        public override IEnumerable<ISerializeScorer> SerializeScorers
        {
            get
            {
                yield return BinaryTypeScorer;
            }
        }
        public override IEnumerable<IDeserializeScorer> DeserializeScorers
        {
            get
            {
                yield return BinaryTypeScorer;
            }
        }

        public virtual BinaryTypeScorer BinaryTypeScorer => Singleton<BinaryTypeScorer>.Instance;

        #endregion

    }

    public class BinaryTypeScorer : ISerializeScorer, IDeserializeScorer
    {
        public float PassScore { get; set; } = 10f;
        public float FailScore { get; set; } = -10f;

        public float ScoreForStrategy(SerializationStrategyPreference preference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null)
            => operation.Value.Type == typeof(byte[]) ? PassScore : FailScore;
    }
}