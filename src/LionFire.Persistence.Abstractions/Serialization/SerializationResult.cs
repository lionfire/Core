using System.Collections.Generic;

namespace LionFire.Serialization
{
    public enum SerializationResultKind
    {
        Unspecified = 0,
        Success = 1 << 0,
        SyntaxError = 1 << 1,
        TypeError = 1 << 2,
        Unrecognized = 1 << 3,
        NotSupported = 1 << 4,
    }

    public class SerializationResult
    {
        public static readonly SerializationResult NotSupported = new SerializationResult { Result = SerializationResultKind.NotSupported };
        public static readonly SerializationResult Success = new SerializationResult { Result = SerializationResultKind.Success };

        public bool IsSuccess => Result == SerializationResultKind.Success;

        public SerializationResultKind Result { get; set; }
        public string Message { get; set; }
        public int ErrorOffset { get; set; }
        public int ErrorLength { get; set; }
        public int ErrorLineLocation { get; set; }
        public int ErrorLineNumber { get; set; }

        public IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> AggregateResults { get; set; }
    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
