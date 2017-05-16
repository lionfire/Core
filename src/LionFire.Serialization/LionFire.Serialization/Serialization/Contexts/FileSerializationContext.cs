using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Serialization.Contexts
{
    public class FileSerializationContext : SerializationContext
    {
        public override IEnumerable<DeserializationStrategy> DeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.FileName;
            }
        }

        #region FileName

        /// <summary>
        /// May be a full path or just a file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// For compressors/decompressors, this is the recommended file name of the output
        /// </summary>
        public string OutputFileName { get; set; }

        #region Derived

        public virtual string FileExtension
        {
            get
            {
                if (FileName == null) return null;
                return Path.GetExtension(FileName);
            }
        }

        #endregion

        #endregion
    }
}
