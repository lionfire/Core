using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;

namespace LionFire.ObjectBus.Postgres
{
    [LionSerializable(SerializeMethod.ByValue)]
    public class PostgresReference : LocalReferenceBase, IHas<IOBase>, IHas<IOBus>
    {
        IOBus IHas<IOBus>.Object => ManualSingleton<PostgresOBus>.GuaranteedInstance;
        IOBase IHas<IOBase>.Object => PostgresOBase;
        PostgresOBase PostgresOBase => PostgresOBase.DefaultInstance; // TODO: different hosts

        #region Construction and Implicit Construction

        public PostgresReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public PostgresReference(string path)
        {
            this.Path = path;
        }

        //public PostgresReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator PostgresReference(string path)
        {
            return new PostgresReference(path);
        }

        #endregion

        #region Conversion and Implicit operators

        //public static implicit operator Handle(PostgresReference postgresRef)
        //{
        //    return postgresRef.ToHandle();
        //}

        #endregion

        #region Conversion

        public static void ValidateCanConvertFrom(IReference reference)
        {
            if (reference.Scheme != UriScheme)
            {
                throw new OBusReferenceException("UriScheme not supported");
            }
        }

        public static PostgresReference ConvertFrom(IReference parent)
        {
            PostgresReference fileRef = parent as PostgresReference;

            if (fileRef == null && parent.Scheme == UriScheme)
            {
                fileRef = new PostgresReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

        #region Scheme

        public const string UriScheme = "postgres";
        public const string UriPrefixDefault = "postgres:";
        public static readonly IEnumerable<string> UriSchemes = new string[] { UriScheme };
        public override IEnumerable<string> AllowedSchemes => UriSchemes;

        public override string Scheme => UriScheme;

        #endregion

        public override string Path
        {
            get { return path; }
            set
            {
//#if MONO
                value = value.Replace('\\', '/');
//#else
//                value = value.Replace('/', '\\');
//#endif

                //if (value != null)
                //{
                //    if (value.Length >= 1)
                //    {
                //        if (value[0] == ':') throw new ArgumentException();
                //    }

                //    var colon = value.LastIndexOf(':');
                //    if (colon != -1 && colon != 1)
                //    {
                //        throw new ArgumentException();
                //    }
                //}

                path = value;
            }
        }
        private string path;

        //public override IOBaseProvider DefaultObjectStoreProvider
        //{
        //    get
        //    {
        //        return PostgresOBaseProvider.Instance;
        //    }
        //}

        public override string ToString() => String.Concat(UriPrefixDefault, Path);
        
        public override string Key => this.ToString();

    }

    public static class PostgresReferenceExtensions
    {
        public static PostgresReference AsPostgresReference(this string path)
        {
            return new PostgresReference(path);
        }
    }
}
