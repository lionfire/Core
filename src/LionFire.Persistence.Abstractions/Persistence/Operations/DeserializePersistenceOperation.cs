using LionFire.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LionFire.Persistence
{
    
    public class DeserializePersistenceOperation
    {
        //public PersistenceDirection PersistenceDirection => PersistenceDirection.Deserialize;
        public DeserializePersistenceOperation() { }

        // TODO: ENH Prevent setting too many
        public IEnumerable<string> CandidatePaths { get; set; }
        //IEnumerable<string> candidatePaths;

        public IEnumerable<string> CandidateFileNames
        {
            get
            {
                if (candidateFileNames != null)
                {
                    return candidateFileNames;
                }

                if (CandidatePaths != null)
                {
                    return CandidatePaths.Select(p => Path.GetFileName(p));
                }
                return null;
            }
            set => candidateFileNames = value;
        }

        public SerializationOptions SerializationOptions { get; set; }

        private IEnumerable<string> candidateFileNames;
        //public string Directory { get; set; }
    }
}
