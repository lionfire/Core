using LionFire.Serialization;
using System.Collections.Generic;
using System.IO;

namespace LionFire.Persistence
{
    public static class DeserializePersistenceOperationExtensions
    {
        public static async IAsyncEnumerable<Stream> CandidateStreams(this PersistenceOperation op, ISerializationStrategy strategy)
        {
            foreach (var path in op.Deserialization.CandidatePaths)
            {
                if (!op.IgnoreFileExtension && !strategy.SupportsExtension(Path.GetExtension(path)))
                {
                    continue;
                }
                
                yield return await op.Context.Deserialization.PathToStream(path).ConfigureAwait(false);
            }
        }
    }
}
