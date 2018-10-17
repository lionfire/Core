using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LionFire.Structures;

namespace LionFire.Serialization.Contexts
{
    public class FileSerializationContext : SerializationContext, IHasPath
    {
        public override IEnumerable<DeserializationStrategy> DefaultDeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.FileName;
                foreach (var item in base.DefaultDeserializationStrategies) yield return item;
            }
        }

        #region FileName

        /// <summary>
        /// May be a full path or just a file name
        /// </summary>
        public string Path { get; set; }


        #region Derived

        public virtual string FileExtension
        {
            get
            {
                if (fileExtension != null) return fileExtension;

                if (Path == null) return null;

                return System.IO.Path.GetExtension(Path);
            }
            set
            {
                fileExtension = value;
            }
        }
        private string fileExtension;

        #endregion

        #endregion


        // OLD / REVIEW Is there a point to this?
        //public override void OnSerialized(ISerializationStrategy s, SerializationFormat format)
        //{
        //    if (FileExtension == null)
        //    {
        //        FileExtension = format.DefaultFileExtension;
        //    }
        //}


        #region OLD

        // TODO: MOVE to a separate Pipeline API
        ///// <summary>
        ///// For compressors/decompressors, this is the recommended file name of the output
        ///// </summary>
        //public string OutputFileName { get; set; }


        //public override void LoadStringDataIfNeeded()
        //{
        //    if (StringData != null) return;

        //    if (string.IsNullOrWhiteSpace(Path)) throw new ArgumentNullException(nameof(Path));

        //    StringData = File.ReadAllText(Path);
        //}

        //public override void LoadBytesDataIfNeeded()
        //{
        //    if (BytesData != null) return;

        //    if (string.IsNullOrWhiteSpace(Path)) throw new ArgumentNullException(nameof(Path));

        //    BytesData = File.ReadAllBytes(Path);
        //}

        #endregion
    }
}
