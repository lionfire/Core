//#define TEMP_ONE_PACKAGE // TEMP until packaging/Vos is debugged/finalized

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonExSerializer;
using LionFire.LionRing;
using LionFire.ObjectBus;
using LionFire.Serialization;

namespace LionFire.Assets
{

    // REFACTOR - version info into this class?
    //public class AssetVersion
    //{

    //}

    /// <summary>
    /// Default paths are not incorporated here
    /// </summary>
    [JsonConvert(typeof(AssetIDSerializationConverter))]
#if !AOT
    [LionSerializable(SerializeMethod.ByValue)]
#endif
    public class AssetID : IHasPath, IFreezable
    {
        #region (Public) Constants

        public const string UriSchemePrefix = "asset:";
        public const string NullPath = "<<null>>";

        public const char AssetPathSeparator = '.';
        public const char PackagePathSeparator = ':';

        #endregion

        #region Construction

        // FUTURE: in AssetID ctor, AutoGenerate revision/build numbers
        public AssetID()
        {
            //this.Package = AssetContext.Current.DefaultCreatePackage; OLD - don't do this.  packages should be null when loading.  Use DefaultSavePackage instead when saving
        }

        public AssetID(string path = NullPath,
            short major = 1,
            short minor = 0,
            short revision = 0,
            short build = 0,
            string publicKey = null
            )
        {
            if (path != null && path.Contains(PackagePathSeparator))
            {
                int packageSeparatorIndex = path.LastIndexOf(PackagePathSeparator);
                this.PackageName = path.Substring(0, packageSeparatorIndex); //UNTESTED
                this.path = path.Substring(packageSeparatorIndex + 1);
            }
            else
            {
                //this.packageName = null;
                this.path = path;
                this.Package = AssetContext.Current.DefaultCreatePackage;
            }

            //var kvp = DirectoryAndNameFromPath(path);
            //Directory = kvp.Key;
            //name = kvp.Value;

            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
            PublicKey = publicKey;
        }


        public static implicit operator AssetID(string assetPath)
        {
            return new AssetID(assetPath);
        }

        #endregion

        #region UNUSED

        //public static readonly AssetID Null = new AssetID(NullPath);

        #region VobHandle


        //public VobHandle<T> ToVosHandle<T>()
        //    where T : class, new()
        //{

        //    return new VobHandle<T>(VosApp.Instance.ActiveData, Path);
        //}

        //public VobHandle<T> ToVosHandleInPackage<T>()
        //        where T : class, new()
        //{
        //    return new VobHandle<T>(VosApp.Instance.ActiveData, Path);
        //}

        #endregion

        #region HAsset

        //public HAsset<T> ToHAsset<T>()
        //        where T : class
        //{
        //    var result = new HAsset<T>(Path);

        //    result.ObjectField = 
        //}

        #endregion

        #endregion

        //public bool IsNull { get { return Path == NullPath; } }


        #region Properties

        #region Path

        public string Path
        {
            //get { return Directory + Name; }
            get { return path; }
            //set
            //{
            //    if (value.Contains(AssetPathSeparator))
            //    {
            //        int nameIndex = value.LastIndexOf(AssetPathSeparator) + 1;
            //        Directory = value.Substring(0, nameIndex);
            //        Name = value.Substring(nameIndex);
            //    }
            //    else
            //    {
            //        Directory = null;
            //        Name = value;
            //    }
            //}
            set
            {
                if (path != null) throw new AlreadyException("Cannot be set more than once");
                path = value;
            }
        }
        private string path;

        #endregion

        #region Package

        [Ignore(LionSerializeContext.Persistence)]
        public string PackageName
        {
            get { return packageName; }
            set
            {
#if TEMP_ONE_PACKAGE
                value = "Nextrek";
#endif
                if (packageName != null && value != packageName) throw new AlreadyException("PackageName can only be set once.");

                packageName = value;

            }
        }
        //public string Name { get { return name; } }
        private string packageName;

        #endregion

        #region Version

        [Ignore(LionSerializeContext.Persistence)]
        public short Major;
        [Ignore(LionSerializeContext.Persistence)]
        public short Minor;
        [Ignore(LionSerializeContext.Persistence)]
        public short Build;
        [Ignore(LionSerializeContext.Persistence)]
        public short Revision;

        [Ignore(LionSerializeContext.Network)]
        [SerializeDefaultValue(false)]
        public string VersionString
        {
            get
            {
                return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
            }
            set
            {
                string[] versionStrings = value.Split('.');
                if (versionStrings.Length >= 1) Major = short.Parse(versionStrings[0]);
                if (versionStrings.Length >= 2) Minor = short.Parse(versionStrings[1]);
                if (versionStrings.Length >= 3) Build = short.Parse(versionStrings[2]);
                if (versionStrings.Length >= 4) Revision = short.Parse(versionStrings[3]);
                if (versionStrings.Length > 4) throw new Exception("Too many .'s in version string.");
            }
        }

        #endregion

        [SerializeDefaultValue(false)]
        public readonly string PublicKey;

        #endregion

        public static KeyValuePair<string, string> DirectoryAndNameFromPath(string path)
        {
            KeyValuePair<string, string> kvp;
            if (path.Contains(AssetPathSeparator))
            {
                // REVIEW - what is this for?
                l.Warn("REVIEW - AssetID AssetPathSeparator: " + path);
                int nameIndex = path.LastIndexOf(AssetPathSeparator) + 1;
                kvp = new KeyValuePair<string, string>(path.Substring(0, nameIndex), path.Substring(nameIndex));
            }
            else
            {
                kvp = new KeyValuePair<string, string>(null, path);
            }
            return kvp;
        }


        #region Derived Properties


        /// <summary>
        /// FUTURE: For now, has no version
        /// </summary>
        [Ignore]
        public string Key
        {
            get
            {
                if (PackageName != null)
                {
                    return PackageName + PackagePathSeparator + Path;
                }
                else
                {
                    return Path;
                }
            }
        }

        #region Derived from Path

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(DirectoryAndNameFromPath(Path).Value); // MICROOPTIMIZE
            }
        }

        public string Directory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(DirectoryAndNameFromPath(Path).Value); // MICROOPTIMIZE
            }
        }

        #region Package

        [Ignore]
        public Package Package
        {
            get
            {
                if (_package == null)
                {
                    if (PackageName != null)
                    {
                        Package = AssetManager<Package>.Load(new AssetID(PackageName) { PackageName = PackageName });
                    }
                }
                return _package;
            }
            set
            {
                if (value != null)
                {
                    PackageName = value.ID.Name; // Throws if already set to something different.
                }
                else
                {
                    PackageName = null;
                }
                _package = value;
            }
        }
        private Package _package;

        #endregion

        #endregion

        #endregion


        #region (Public) Methods


        public AssetID Clone()
        {
            return new AssetID
            {
                path = this.path,
                _package = _package,
                Major = Major,
                Minor = Minor,
                Revision = Revision,
                Build = Build,
                //PublicKey = PublicKey
            };
        }

        #region Freezing

        [Ignore]
        public bool IsFrozen
        {
            get
            {
                return path != null;
            }
            set
            {
                if (value == isFrozen) return;
                if (!value) throw new Exception("Cannot unfreeze");

                if (path != null) throw new Exception("Path must be set before freezing");
                isFrozen = value;
            }
        }
        private bool isFrozen;

        #endregion

        #endregion

        #region Misc

        #region Identity Comparison


        public override bool Equals(object obj)
        {
            AssetID other = obj as AssetID;
            if (other == null) return this.Key == null;
            if (this.Key == null) return false;
            return this.Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion


        public override string ToString()
        {
            return (PackageName == null ? "" : PackageName + PackagePathSeparator) + Path;
        }

        public string ToUriString()
        {
            return UriSchemePrefix + ToString();
        }

        private static ILogger l = Log.Get();

        #endregion

    }
}
