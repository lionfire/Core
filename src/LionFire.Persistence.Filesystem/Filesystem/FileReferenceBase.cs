using System;
using System.Collections.Generic;
using LionFire.ObjectBus;
using LionFire.ObjectBus.Filesystem;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;

namespace LionFire.Persistence.Filesystem
{
    [LionSerializable(SerializeMethod.ByValue)]
    public abstract class FileReferenceBase<ConcreteType> : LocalReferenceBase<ConcreteType>
        //, IHas<IOBase>, IHas<IOBus>
        //, IOBaseReference
        where ConcreteType : FileReferenceBase<ConcreteType>
    {
        public abstract string UriPrefixDefault { get; }
        public abstract string UriSchemeColon { get; }
        public abstract string UriScheme { get; }
        public override IEnumerable<string> AllowedSchemes { get { yield return UriScheme; } }


        //IOBus IHas<IOBus>.Object => ManualSingleton<FSOBus>.GuaranteedInstance;
        //IOBase IHas<IOBase>.Object => FsOBase;
        //FSOBase FsOBase => ManualSingleton<FSOBase>.GuaranteedInstance;

        public FileReferenceBase() { }
        public FileReferenceBase(string path)
        {
            this.Path = path;
        }

        #region Conversion and Implicit operators

        //public static implicit operator Handle(FileReference fsref)
        //{
        //    return fsref.ToHandle();
        //}

        #endregion

        public static string CoercePath(string path)
        {
            //#if MONO
            path = path.Replace('\\', '/');
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

            return path;
        }

        protected override void InternalSetPath(string path)
        {
            this.path = CoercePath(path);
        }

        

        public override string Key
        {
            get => string.Concat(UriPrefixDefault, Path);
            protected set
            {
                if (value.StartsWith(UriSchemeColon))
                {
                    if (value.StartsWith(UriPrefixDefault))
                    {
                        value = value.Substring(UriPrefixDefault.Length);
                    }
                    else
                    {
                        value = value.Substring(UriSchemeColon.Length);
                    }
                }
                Path = value;
            }
        }


        public  string ToXamlString() => $"{{{this.GetType().Name} {Path}}}";
        public override string ToString() => Key; 

    }

}
