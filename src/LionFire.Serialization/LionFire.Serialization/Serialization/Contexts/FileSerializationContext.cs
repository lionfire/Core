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


        public override void LoadStringDataIfNeeded()
        {
            if (StringData != null) return;

            if (string.IsNullOrWhiteSpace(FileName)) throw new ArgumentNullException(nameof(FileName));

            StringData = File.ReadAllText(FileName);
        }

        public override void LoadBytesDataIfNeeded()
        {
            if (BytesData != null) return;

            if (string.IsNullOrWhiteSpace(FileName)) throw new ArgumentNullException(nameof(FileName));

            BytesData = File.ReadAllBytes(FileName);
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
                if (fileExtension != null) return fileExtension;

                if (FileName == null) return null;

                return Path.GetExtension(FileName);
            }
            set
            {
                fileExtension = value;
            }
        }
        private string fileExtension;

        #endregion

        #endregion


        public override void OnSerialized(ISerializerStrategy s)
        {
            if (FileExtension == null)
            {
                FileExtension = s.DefaultFileExtension;
            }
        }
    }
}
