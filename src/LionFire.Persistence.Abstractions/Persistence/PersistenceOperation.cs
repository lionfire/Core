using System;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Persistence
{
    public class PersistenceOperation
    {
        public static implicit operator Lazy<PersistenceOperation>(PersistenceOperation op) => new Lazy<PersistenceOperation>(() => op);
        public static implicit operator PersistenceOperation(Lazy<PersistenceOperation> op) => (PersistenceOperation)op.Value;

        public PersistenceDirection Direction
        {
            get
            {
                if (direction.HasValue)
                {
                    return direction.Value;
                }

                if (Serialization != null)
                {
                    return PersistenceDirection.Serialize;
                }

                if (Deserialization != null)
                {
                    return PersistenceDirection.Deserialize;
                }

                return PersistenceDirection.Unspecified;
            }
            set => direction = value;
        }
        private PersistenceDirection? direction;

        public PersistenceContext Context { get; set; } 

        #region Type

        /// <summary>
        /// Deserialization: deserialize to this type.
        /// Serialization: use this type as a hint or guidance for serialization.
        /// </summary>
        public Type Type { get; set; }

        public string MimeType { get; set; }

        #endregion

        #region Reference / Path

        public IReference Reference { get; set; }

        #region Path

        /// <seealso cref="PersistenceOperationExtensions.SetPath"/>
        public string Path
        {
            get => Reference?.Path;
        }
        public bool? PathIsMissingExtension { get; set; }

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

        #region TODO: MultiTyped extensibility

        ///// <summary>
        ///// Arbitrary contextual information that may be useful.  
        ///// </summary>
        ////[MultiType]
        //public object OperationContext { get; set; } // UNUSED 

        ///// <summary>
        ///// Arbitrary contextual information that may be useful
        ///// </summary>
        //public object MetaData { get; set; }  // UNUSED

        #endregion

        public SerializePersistenceOperation Serialization { get; set; }
        public DeserializePersistenceOperation Deserialization { get; set; }
        //private PersistenceOperation persistenceOperation; // REVIEW - Use one backing field for both?

        #region Options

        public bool IgnoreFileExtension { get; set; }
        public bool IgnoreMimeType { get; set; } // UNUSED

        #endregion
    }

}
