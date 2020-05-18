using System;
using System.Linq;
using LionFire.IO;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    // FUTURE: Make this some sort of executable/awaitable pipeline type thing?
    // REVIEW - best way to create these operations, and if they are even useful.  Also, if so, maybe jsut use this to pass the PersistenceContext

    /// <summary>
    /// REVIEW: What use is this, or what use might it be?  It could act as a blackboard for every mechanism involved in persistence and serialization
    /// </summary>
    public class PersistenceOperation
    {
        public static implicit operator Lazy<PersistenceOperation>(PersistenceOperation op) => new Lazy<PersistenceOperation>(() => op);
        public static implicit operator PersistenceOperation(Lazy<PersistenceOperation> op) => (PersistenceOperation)op?.Value;

        #region Static



        //public static PersistenceOperation CreateReadOperation(IReference reference = null) => new PersistenceOperation(reference)
        //{
        //    Reference = reference,
        //    Direction = IODirection.Read,
        //};

        //public static readonly PersistenceOperation DefaultReadOperation = new PersistenceOperation()
        //{
        //    Direction = IODirection.Read,
        //};
        //public static readonly PersistenceOperation DefaultWriteOperation = new PersistenceOperation()
        //{
        //    Direction = IODirection.Write,
        //};

        #endregion

        #region Construction

        public PersistenceOperation() { }
        public PersistenceOperation(IReference reference)
        {
            this.Reference = reference;
        }
        public PersistenceOperation(IReference reference, SerializePersistenceOperation serialization) : this(reference)
        {
            this.Serialization = serialization;
        }
        public PersistenceOperation(IReference reference, DeserializePersistenceOperation deserialization) : this(reference)
        {
            this.Deserialization = deserialization;
        }

        #endregion

        public IODirection Direction
        {
            get
            {
                if (direction.HasValue)
                {
                    return direction.Value;
                }

                if (Serialization != null)
                {
                    return IODirection.Write;
                }

                if (Deserialization != null)
                {
                    return IODirection.Read;
                }

                return IODirection.Unspecified;
            }
            set => direction = value;
        }
        private IODirection? direction;

        public PersistenceContext Context { get; set; }

        #region Type

        /// <summary>
        /// Deserialization: deserialize to this type.
        /// Serialization: use this type as a hint or guidance for serialization.
        /// </summary>
        public Type Type { get; set; }

        public string MimeType { get; set; }

        #endregion

        public LionSerializeContext SerializeContext
        {
            get
            {
                if (serializeContext.HasValue) return serializeContext.Value;
                //if (Reference.Host.IsRemote()) { return LionSerializeContext.Network; } // FUTURE
                return LionSerializeContext.Persistence;
            }
        }
        private LionSerializeContext? serializeContext;

        #region Reference / Path

        public IReference Reference { get; set; }

        #region Path

        /// <seealso cref="PersistenceOperationExtensions.SetPath"/>
        public string Path
        {
            get => Reference?.Path;
        }
        //public bool? PathIsMissingExtension { get; set; } // TODO REVIEW - is this still needed now that there is AutoAppendExtension?

        public AutoAppendExtension? AutoAppendExtension { get; set; }

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

        public SerializationOptions SerializationOptions => Deserialization?.SerializationOptions ?? Serialization?.SerializationOptions;

        //private PersistenceOperation persistenceOperation; // REVIEW - Use one backing field for both?

        #region Options

        public bool IgnoreFileExtension { get; set; }
        public bool IgnoreMimeType { get; set; } // UNUSED

        #region FileExtension

        public string FileExtension
        {
            get => fileExtension ?? LionPath.GetExtension(Reference?.Path) ?? SerializationOptions?.TreatExtensionlessAsExtension;
            set => fileExtension = value;
        }
        protected string fileExtension;

        #endregion


        #endregion

    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}

}
