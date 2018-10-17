using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Serialization
{
    public class SerializationOperation
    {
        /// <summary>
        /// Deserialization: deserialize to this type.
        /// Serialization: use this type as a hint or guidance for serialization.
        /// </summary>
        public Type Type { get; set; }

        public string MimeType { get; set; }

        #region Reference / Path

        public IReference Reference { get; set; }

        #region Path

        public string Path
        {
            get => Reference?.Path;
            set => Reference = (PathReference)value;
        }

        #endregion

        #region Extension

        public string Extension
        {
            get
            {
                if (extension != null)
                {
                    return extension;
                }
                else if (Path != null)
                {
                    return System.IO.Path.GetExtension(Path);
                }
                return null;
            }
            set => extension = value;
        }
        private string extension;

        #endregion

        #endregion

        /// <summary>
        /// Arbitrary contextual information that may be useful.  
        /// </summary>
        //[MultiType]
        public object OperationContext { get; set; } // UNUSED 

        /// <summary>
        /// Arbitrary contextual information that may be useful
        /// </summary>
        public object MetaData { get; set; }  // UNUSED
    }

    /// <summary>
    /// REVIEW:
    ///  - reduce the footprint of this?
    /// - REVIEW: If this is going to be a parameter object, make it a struct?
    /// </summary>
    public class SerializationContext
    {
        #region FUTURE ?

        //public bool RequireExactType { get; set; }

        #endregion

        public SerializerSelectionContext SerializerSelectionContext { get; set; }

        public object SerializerOptions { get; set; }

        public SerializationFlags Flags { get; set; }

        #region DeserializationStrategies

        public virtual IEnumerable<DeserializationStrategy> DefaultDeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.Detect;
            }
        }
        public IEnumerable<DeserializationStrategy> DeserializationStrategies
        {
            get => deserializationStrategies ?? DefaultDeserializationStrategies; set => deserializationStrategies = value;
        }

        #region Encoding

        public Encoding Encoding { get; set; }

        #endregion

        private IEnumerable<DeserializationStrategy> deserializationStrategies;

        #endregion

        public virtual void OnSerialized(ISerializationStrategy s) { }

        // OLD - nope
        //public virtual void LoadStringDataIfNeeded() { }
        //public virtual void LoadBytesDataIfNeeded() { }
        // TODO: Throw if can't resolve StringData / BytesData

    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
